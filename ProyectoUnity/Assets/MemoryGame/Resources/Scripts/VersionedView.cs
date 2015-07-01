using UnityEngine;
using System.Collections;

public class VersionedView : MonoBehaviour , IVersioned
{
	ulong cachedVersion =0;
	ulong version = 0;

	// Update is called once per frame
	protected virtual void Update ()
	{
		if(cachedVersion != Version)
		{
			cachedVersion = Version;
			DirtyUpdate();
		}
	}

	#region IVersioned implementation

	public void MarkDirty ()
	{
		Version++;
	}

	public virtual void DirtyUpdate ()
	{
	}

	public ulong Version {
		get 
		{
			return version;
		}
		set 
		{
			version = value;
		}
	}

	#endregion
}
