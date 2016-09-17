using System;

namespace BuildHelper.Log
{
	interface IExtensionLogger
	{
		void WriteStatus(string message);

		void WriteException(Exception exception);
	}
}
