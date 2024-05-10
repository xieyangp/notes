## 使用ffmpeg合并视频
```c#
 private async Task<string> CombineMeetingRecordVideoAsync(Guid meetingId, Guid meetingRecordId, string egressUrl, CancellationToken cancellationToken)
    {
        var urls = (await _meetingDataProvider.GetMeetingRecordVoiceRelayStationAsync(
                meetingId, meetingRecordId, cancellationToken).ConfigureAwait(false))
            .Where(x => !x.Url.IsNullOrEmpty())
            .Select(x => x.Url).ToList();
        
        if (urls is { Count: 0 }) 
            return null;
        
        urls.Add(egressUrl);
        
        Log.Information("Combine urls: {@urls}", urls);
        
        var urlBytes = new List<byte[]>();
        
        foreach (var url in urls)
        {
            var urlContent = await _httpClientFactory.GetAsync<byte[]>(url, cancellationToken).ConfigureAwait(false);

            urlBytes.Add(urlContent);
        }
        
        try
        {
            //调用拼接视频的方法
            var response = await _ffmpegService.CombineMp4VideosAsync(urlBytes, cancellationToken).ConfigureAwait(false);
            
            Log.Information("Combined Videos response : {@response}", response);

            var fileName = $"SugarTalk/{Guid.NewGuid()}.mp4";
            
            _aliYunOssService.UploadFile(fileName, response);
            
            var url = _aliYunOssService.GetFileUrl(fileName);
            var indexOfQuestionMark = url.IndexOf('?');

            if (indexOfQuestionMark == -1) return null;
            
            var extractedUrl = url[..indexOfQuestionMark];
            
            return extractedUrl;
        }
        catch (Exception ex)
        {
            Log.Error(ex, @"Combine url upload failed, {urls}", JsonConvert.SerializeObject(urls)); 
            throw;
        }
    }
```
----
### 代码
```c#
   public async Task<byte[]> CombineMp4VideosAsync(List<byte[]> videoDataList, CancellationToken cancellationToken = default)
    {
        try
        {
            var outputFileName = $"{Guid.NewGuid()}.mp4";
            var inputFiles = "";
            
            var downloadedVideoFiles = new List<string>();
            foreach (var videoData in videoDataList)
            {
                var videoFileName = $"{Guid.NewGuid()}.mp4";
                await File.WriteAllBytesAsync(videoFileName, videoData, cancellationToken).ConfigureAwait(false);
                downloadedVideoFiles.Add(videoFileName);
                inputFiles += $"-i \"{videoFileName}\" ";
            }

            var filterComplex = $"-filter_complex \"";

            for (int i = 0; i < downloadedVideoFiles.Count; i++)
            {
                filterComplex += $"[{i}:v:0][{i}:a:0]";
            }
            
            filterComplex += $"concat=n={downloadedVideoFiles.Count}:v=1:a=1[outv][outa]\"";

            var combineArguments = $"{inputFiles} {filterComplex} -map \"[outv]\" -map \"[outa]\" {outputFileName}";
            Log.Information("Combine command arguments: {combineArguments}", combineArguments);
            
            using (var proc = new Process())
            {
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    Arguments = combineArguments
                };
            
                proc.OutputDataReceived += (_, e) => Log.Information("Combine audio, {@Output}", e);

                proc.Start();
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();

                await proc.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            }
            
            if (File.Exists(outputFileName))
            {
                var resultBytes = await File.ReadAllBytesAsync(outputFileName, cancellationToken).ConfigureAwait(false);

                foreach (var fileName in downloadedVideoFiles)
                {
                    File.Delete(fileName);
                }
                
                File.Delete(outputFileName);

                return resultBytes;
            }

            Log.Error("Failed to generate the combined video file.");
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while combining MP4 videos.");
            return Array.Empty<byte>();
        }
    }
```
