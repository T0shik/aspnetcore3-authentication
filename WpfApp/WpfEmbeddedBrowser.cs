using IdentityModel.OidcClient.Browser;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{
    public class WpfEmbeddedBrowser : IBrowser
    {
        private BrowserOptions _options = null;

        public WpfEmbeddedBrowser()
        {

        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            _options = options;

            var window = new Window()
            {
                Width = 900,
                Height = 625,
                Title = "IdentityServer Demo Login"
            };

            // Note: Unfortunately, WebBrowser is very limited and does not give sufficient information for 
            //   robust error handling. The alternative is to use a system browser or third party embedded
            //   library (which tend to balloon the size of your application and are complicated).
            var webBrowser = new WebBrowser();

            var signal = new SemaphoreSlim(0, 1);

            var result = new BrowserResult()
            {
                ResultType = BrowserResultType.UserCancel
            };

            webBrowser.Navigating += (s, e) =>
            {
                if (BrowserIsNavigatingToRedirectUri(e.Uri))
                {
                    e.Cancel = true;

                    result = new BrowserResult()
                    {
                        ResultType = BrowserResultType.Success,
                        Response = e.Uri.AbsoluteUri
                    };

                    signal.Release();

                    window.Close();
                }
            };

            window.Closing += (s, e) =>
            {
                signal.Release();
            };

            window.Content = webBrowser;
            window.Show();
            webBrowser.Source = new Uri(_options.StartUrl);

            await signal.WaitAsync();

            return result;
        }

        private bool BrowserIsNavigatingToRedirectUri(Uri uri)
        {
            return uri.AbsoluteUri.StartsWith(_options.EndUrl);
        }
    }
}