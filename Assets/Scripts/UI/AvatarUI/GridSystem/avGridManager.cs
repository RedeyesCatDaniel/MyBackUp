using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avGridManager : MonoBehaviour
    {
        //public int row;
        //public int col;
        //public int total;

        public GridLayoutData defaultGridData;
        public bool ConstructOnAwake;
        public avRowManager sampleRow;
        private avRowManager[] rows;
        public avGridElementManager[] allElems;



        private void Awake()
        {
            if(ConstructOnAwake)
                ConstructPageByDefault();
        }


        [ContextMenu(itemName: "Construct")]
        public void ConstructPageByDefault() {
            ConstructPage(defaultGridData.row, defaultGridData.col);
        }

        //this function will construct page
        public void ConstructPage(int row, int col) {
            rows = new avRowManager[row];
            for (int i = 0; i < row; i++)
            {
                rows[i] = ConstructRow(col);
            }

            allElems = GetCurrGridElems();

        }

        private avRowManager ConstructRow(int col) {
            avRowManager row = Instantiate<avRowManager>(sampleRow,transform);
            row.ConstructRow(col);
            row.gameObject.SetActive(true);
            return row;
        }

        public avGridElementManager[] GetCurrGridElems() {
            avGridElementManager[] elems = new avGridElementManager[defaultGridData.GetPageSize()];
            int count = 0;
            foreach (avRowManager row in rows) {
                foreach (avGridElementManager elem in row.GetElems()) {
                    elems[count] = elem;
                    count++;
                }
            }
            return elems;
        }

        public avRowManager[] GetRows() {
            return rows;
        }

        public void HideRow(int index) {
            rows[index].gameObject.SetActive(false);
        }

        public void ShowRow(int index)
        {

            //Debug.Log(index);
            rows[index].gameObject.SetActive(true);
            
        }

        public void ShowRowUtil(int index)
        {
            for (int i = 0; i < Mathf.Min(rows.Length,index); i++)
            {
                ShowRow(i);
            }

            for (int i = index; i< rows.Length;i++) {
                HideRow(i);
            }
        }

        public void ShowAllRow()
        {
            foreach (var item in rows)
            {
                if (!item.isActiveAndEnabled) {
                    item.gameObject.SetActive(true);
                }
            }
        }








    }

    [System.Serializable]
    public struct GridLayoutData
    {
        public int row;
        public int col;

        public int GetPageSize()
        {
            return row * col;

        }
    }
}