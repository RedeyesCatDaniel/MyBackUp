using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avPageManager : MonoBehaviour
    {
        public avGridManager gridman;
        public int total;
        public int pageCount;
        public int currentPage;
        public int pageSize;

        public UnityEvent<int> OnPageTurn;
        public void Init(int total)
        {
            //Debug.Log($"Page Is Initiated by {total}");
            this.total = total;
            pageSize = gridman.defaultGridData.GetPageSize();
            pageCount = GetPageCount(gridman.defaultGridData);
            currentPage = 0;

            UpdateIndex(currentPage);
            TurnToPage(0);
            
        }

        private int GetPageCount(GridLayoutData gridData)
        {
            int onePageCount = gridData.col * gridData.row;
            int pages = total / onePageCount;
            if (total % onePageCount > 0)
            {
                pages++;
            }
            return pages;
        }

        public int TurnToPage(int target) {
            int finalPage = Mathf.Min(target, pageCount-1);
            finalPage = Mathf.Max(0, finalPage);

            //calculate how many pages are shown
            int count = GetPageElemCount(target);
            Cutpage(count, finalPage);

            if (currentPage != finalPage) {
                currentPage = finalPage;
                //reindex each page
                
                OnPageTurn.Invoke(currentPage);
            }

            
            return finalPage;
        }

        private void UpdateIndex(int currentPage) {
            for (int i = 0; i < gridman.defaultGridData.GetPageSize(); i++)
            {
                avGridElementManager man = gridman.allElems[i];
                man.eleIndex = i + (currentPage) * gridman.defaultGridData.GetPageSize();
               // Debug.Log($"{man.eleIndex} = {i}+({currentPage}+1)*{gridman.defaultGridData.GetPageSize()})");
            }
        }

        //this function will split the page into active and inactive according to the number of grid
        //on this page
        private void Cutpage(int count,int currentPage) {
            //  Debug.Log($"I need to turn on {count} grid");
            for (int i = 0; i < gridman.defaultGridData.GetPageSize(); i++)
            {
                avGridElementManager man = gridman.allElems[i];
               // man.eleIndex = i + (currentPage) * pageCount;
                man.TurnOff();
            }

            UpdateIndex(currentPage);

            for (int i = 0; i < count; i++)
            {
                
                avGridElementManager man = gridman.allElems[i];
                
                man.TurnOn();
            }

            
        }

        private int GetPageElemCount(int page) {
            if (total == 0) {
                return 0;
            }
            int pageCount = gridman.defaultGridData.GetPageSize();
            if ((page+1) * pageCount >= total)
            {
                int rs = total % pageCount;
                if (rs == 0) {
                    rs = pageCount;
                }
                return rs;
            }
            else {
                return pageCount;
            }
        }

        public void TurnToNextPage() {
            if (currentPage < pageCount) {
                TurnToPage(currentPage + 1);
            }
        }

        public void TurnToLastPage()
        {
            if (currentPage > 0)
            {
                TurnToPage(currentPage - 1);
            }
        }

        public avGridElementManager GetGridElement(int index) {
            index = index % pageSize;
            return gridman.allElems[index];
        }

        public void ShowRowUtil(int index) {
            gridman.ShowRowUtil(index);
        }



    }
}