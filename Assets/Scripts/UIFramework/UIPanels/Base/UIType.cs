using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIType
{
	/// <summary>
	/// UI的名称
	/// </summary>
   public string UIName { get; private set; }
	/// <summary>
	/// 预制体位于UI文件夹下的具体路径
	/// </summary>
	public string Path { get; private set; }

	public UIType(string Path)
	{
		this.Path = Path;
		UIName = Path.Substring(Path.LastIndexOf('/') + 1);
	}
}
