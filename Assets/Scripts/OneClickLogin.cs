using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LGUVirtualOffice
{
	public class OneClickLogin : MonoBehaviour
	{
		public Button LoginBtn;
		public TMP_InputField input;
		public TextMeshProUGUI Team;
		string Name1 = "이승원";
		string Name2 = "이준제";
		string TeamName = "언택트서비스TF";

		public void OneClickLoginBtn(int id)
		{
			if(id==0)
			input.text = Name1;
			else if(id==1)
				input.text = Name2;
			Team.text = TeamName;
			LoginBtn.onClick.Invoke();
		}
	}
}
