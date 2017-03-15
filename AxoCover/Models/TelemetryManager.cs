﻿using AxoCover.Common.Extensions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AxoCover.Models
{
  public abstract class TelemetryManager : ITelemetryManager
  {
    protected IEditorContext _editorContext;
    protected IOptions _options;

    public bool IsTelemetryEnabled
    {
      get { return _options.IsTelemetryEnabled; }
      set { _options.IsTelemetryEnabled = value; }
    }

    public TelemetryManager(IEditorContext editorContext, IOptions options)
    {
      _editorContext = editorContext;
      _options = options;

      Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      var description = e.Exception.GetDescription();
      if (description.Contains(nameof(AxoCover)))
      {
        _editorContext.WriteToLog(Resources.ExceptionEncountered);
        _editorContext.WriteToLog(description);
        if (!Debugger.IsAttached)
        {
          UploadExceptionAsync(e.Exception);
          e.Handled = true;
        }
      }
    }

    public abstract Task<bool> UploadExceptionAsync(Exception exception);
  }
}
