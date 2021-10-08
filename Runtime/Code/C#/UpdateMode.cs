using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public enum UpdateMode
	{
		NO_UPDATE = 0,
		UPDATE,
		FIXED_UPDATE,
		LATE_UPDATE,
	}

	public enum UpdateModeRequired
	{
		UPDATE,
		FIXED_UPDATE,
		LATE_UPDATE
	}
}
