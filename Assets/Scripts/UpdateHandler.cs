using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHandler : MonoBehaviour
{
    AppUpdateManager appUpdateManager;

    public void Start()
    {
        appUpdateManager = new AppUpdateManager();
        StartCoroutine(CheckForUpdate());
    }

    IEnumerator CheckForUpdate()
    {
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          appUpdateManager.GetAppUpdateInfo();

        // Wait until the asynchronous operation completes.
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            if(appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable) 
            {
                // Creates an AppUpdateOptions defining an immediate in-app
                // update flow and its parameters.
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                StartCoroutine(StartImmediateUpdate(appUpdateInfoResult, appUpdateOptions));
            }
            // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
            // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
            // to start an in-app update.
        }
        else
        {
            // Log appUpdateInfoOperation.Error.
        }
    }

    private void BeginUpdate()
    {
        
    }

    IEnumerator StartImmediateUpdate(AppUpdateInfo info, AppUpdateOptions options)
    {
        // Creates an AppUpdateRequest that can be used to monitor the
        // requested in-app update flow.
        var startUpdateRequest = appUpdateManager.StartUpdate(
          // The result returned by PlayAsyncOperation.GetResult().
          info,
          // The AppUpdateOptions created defining the requested in-app update
          // and its parameters.
          options);
        yield return startUpdateRequest;

        // If the update completes successfully, then the app restarts and this line
        // is never reached. If this line is reached, then handle the failure (for
        // example, by logging result.Error or by displaying a message to the user).
    }

}
