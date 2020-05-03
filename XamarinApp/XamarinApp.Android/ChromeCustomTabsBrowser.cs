using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.CustomTabs;
using IdentityModel.OidcClient.Browser;
using Plugin.CurrentActivity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace XamarinApp.Droid
{
    public class ChromeCustomTabsBrowser : IBrowser
    {
        private readonly Activity _context;
        private readonly CustomTabsActivityManager _manager;

        public ChromeCustomTabsBrowser() : this(CrossCurrentActivity.Current.Activity) { }


        public ChromeCustomTabsBrowser(Activity context)
        {
            _context = context;
            _manager = new CustomTabsActivityManager(_context);
        }

        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            var task = new TaskCompletionSource<BrowserResult>();

            var builder = new CustomTabsIntent.Builder(_manager.Session)
               .SetToolbarColor(Color.Argb(255, 52, 152, 219))
               .SetShowTitle(true)
               .EnableUrlBarHiding();

            var customTabsIntent = builder.Build();

            // ensures the intent is not kept in the history stack, which makes
            // sure navigating away from it will close it
            customTabsIntent.Intent.AddFlags(ActivityFlags.NoHistory);

            Action<string> callback = null;
            callback = url =>
            {
                OidcCallbackActivity.Callbacks -= callback;

                task.SetResult(new BrowserResult()
                {
                    Response = url
                });
            };

            OidcCallbackActivity.Callbacks += callback;

            customTabsIntent.LaunchUrl(_context, Android.Net.Uri.Parse(options.StartUrl));

            return task.Task;
        }
    }
}