using System;
using System.IO;
using System.Net;
using System.Text;

namespace CapriDesktop
{
    internal static class VoiceStudio
    {
        internal static string Body(string text, string profile)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length > 4096) throw new VoiceException("VoiceStudio needs between 1 and 4096 characters per reply.");
            return "{\"model\":\"tts-1\",\"voice\":" + Voice.Quote(profile) + ",\"input\":" + Voice.Quote(text) + ",\"response_format\":\"wav\"}";
        }

        internal static byte[] Speak(string text, string profile)
        {
            byte[] body = Encoding.UTF8.GetBytes(Body(text, profile));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:3900/v1/audio/speech");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer local";
            request.Proxy = null;
            request.AllowAutoRedirect = false;
            request.Timeout = request.ReadWriteTimeout = 120000;
            request.ContentLength = body.Length;
            try
            {
                using (Stream stream = request.GetRequestStream()) stream.Write(body, 0, body.Length);
                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (MemoryStream audio = new MemoryStream())
                {
                    stream.CopyTo(audio);
                    byte[] wav = audio.ToArray();
                    if (wav.Length < 44 || Encoding.ASCII.GetString(wav, 0, 4) != "RIFF" || Encoding.ASCII.GetString(wav, 8, 4) != "WAVE") throw new VoiceException("VoiceStudio returned invalid WAV audio.");
                    return wav;
                }
            }
            catch (WebException error)
            {
                if (error.Response != null) error.Response.Close();
                throw new VoiceException("VoiceStudio couldn't speak. Open VoiceStudio, load a speech model and check your voice profile ID.");
            }
            catch (IOException) { throw new VoiceException("VoiceStudio's audio connection stopped. Try again."); }
        }
    }
}
