using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace MyWebApp.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string VideoUrl { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(VideoUrl))
            {
                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(VideoUrl);

                if (video != null)
                {
                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                    var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

                    if (streamInfo != null)
                    {
                        using var httpClient = new HttpClient();
                        var videoBytes = await httpClient.GetByteArrayAsync(streamInfo.Url);

                        if (videoBytes != null && videoBytes.Length > 0)
                        {
                            return File(videoBytes, "video/mp4", $"{video.Id}.mp4");
                        }
                    }
                }
            }

            // Si no se puede procesar el URL, redirigir al usuario de vuelta a la página
            return RedirectToPage();
        }
    }
}