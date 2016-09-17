using System;
using EnsureThat;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildHelper.Log
{
	internal class ExtensionLogger : IExtensionLogger
	{
		private readonly Func<IVsActivityLog> _logProvider;
		private readonly IVsStatusbar _statusBar;

		public ExtensionLogger(Func<IVsActivityLog> logProvider, IVsStatusbar statusBar)
		{
			Ensure.That(() => logProvider).IsNotNull();
			Ensure.That(() => statusBar).IsNotNull();
			_logProvider = logProvider;
			_statusBar = statusBar;
		}

		public void WriteException(Exception exception)
		{
			var log = _logProvider();
			if (log != null)
			{
				log.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
				this.ToString(), "Error has occured: " + exception.ToString());
			}
		}

		public void WriteStatus(string message)
		{
			// Make sure the status bar is not frozen
			int frozen;
			_statusBar.IsFrozen(out frozen);
			if (frozen != 0)
			{
				_statusBar.FreezeOutput(0);
			}

			// Set the status bar text and make its display static.
			_statusBar.SetText(message);

			// Freeze the status bar.
			_statusBar.FreezeOutput(1);

			// Clear the status bar text.
			_statusBar.FreezeOutput(0);
			_statusBar.Clear();
		}
	}
}
