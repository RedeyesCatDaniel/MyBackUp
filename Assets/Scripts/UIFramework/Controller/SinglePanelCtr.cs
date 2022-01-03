using LGUVirtualOffice.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LGUVirtualOffice
{
    public class SinglePanelCtr : AbstractController,IPointerEnterHandler,IPointerExitHandler
    {
		IUIService UIService;
		BasePanel basePanel;

		public void Initailize(BasePanel basePanel)
		{
			UIService = this.GetService<IUIService>();
			this.basePanel = basePanel;


		}

		public void OnPointerExit(PointerEventData eventData)
		{

		}

		public void OnPointerEnter(PointerEventData eventData)
		{

		}
	}
}

