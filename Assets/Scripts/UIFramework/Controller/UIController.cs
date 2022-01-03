using LGUVirtualOffice.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace LGUVirtualOffice
{
    public class UIController : AbstractController
    {
		IUIService UIService;
        public List<Transform> Canvas;

		public void Start()
		{
			UIService = this.GetService<IUIService>();

			UIService.SetCanvasList(Canvas);
		}
	}
}

