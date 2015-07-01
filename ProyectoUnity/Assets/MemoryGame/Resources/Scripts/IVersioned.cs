using UnityEngine;
using System.Collections;

public interface IVersioned 
{
	void MarkDirty();
	void DirtyUpdate();
	ulong Version{get;set;}
}
