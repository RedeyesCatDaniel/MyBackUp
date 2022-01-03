using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    public class avRowManager : MonoBehaviour
    {
        public avGridElementManager elementSample;
        private avGridElementManager[] elems;
        public avGridElementManager[] ConstructRow(int col) {
            if (elementSample == null) {
                elems = new avGridElementManager[0];
                return elems;
            }
            avGridElementManager[] mans = new avGridElementManager[col];
            
            for (int i = 0; i < col; i++)
            {
                avGridElementManager elem = Instantiate<avGridElementManager>(elementSample, transform);
                mans[i] = elem;
                elem.gameObject.SetActive(true);

            }
            elems = mans;
            return mans;
        }

        public IEnumerable<avGridElementManager> GetElems() {
            return elems;
        }
    }
}
