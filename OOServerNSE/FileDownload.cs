/*
 * OptionsOracle NSE Interface Class Library
 * Copyright 2014 Deepak Shenoy (Capital Mind) and SamoaSky
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO.Compression;

namespace OOServerNSE
{
  public enum DownloadMethod { Get, Post };

  //public delegate void DownloadStateChangeHandler(FileDownload item);
  //public delegate void DownloadAddHandler(FileDownload item);

  //public delegate void DownloadProgressChangedHandler(FileDownload item, int bytesDone, int bytesTotal);

  public class FileDownload 
  {

    #region properties

    // DownloadMethod 
    private DownloadMethod FDownloadMethod = DownloadMethod.Get;
    public DownloadMethod downloadMethod
    {
      get { return FDownloadMethod; }
      set { FDownloadMethod = value; }
    }

    private bool FIsAsyncDownload = true;
    public bool IsAsyncDownload
    {
      get { return FIsAsyncDownload; }
      set { FIsAsyncDownload = value; }
    }

    // Property - URL to be grabbed
    private string FSourceURL;
    public string SourceURL
    {
      get
      {
        return FSourceURL;
      }
      set
      {
        FSourceURL = value;
      }
    }

    private bool FCheckCacheModified = false;
    public bool CheckCacheModified
    {
      get { return FCheckCacheModified; }
      set { FCheckCacheModified = value; }
    }

    private DateTime FCacheLastModified;
    public DateTime CacheLastModified
    {
      get { return FCacheLastModified; }
      set { FCacheLastModified = value; }
    }

    // Property - File to be saved as
    private string FDestinationPath;
    public string DestinationPath
    {
      get
      {
        return FDestinationPath;
      }
      set
      {
        FDestinationPath = value;
      }
    }

    private bool FAddReferrer = false;
    public bool AddReferrer
    {
      get { return FAddReferrer; }
      set { FAddReferrer = value; }
    }
    public string Referrer;

    // Content-type to be passed for POST
    private string FContentType;
    public string ContentType
    {
      get
      {
        return FContentType;
      }
      set
      {
        FContentType = value;
      }
    }

    private int FbytesDone;

    public int bytesDone
    {
      get { return FbytesDone; }
      set { FbytesDone = value; }
    }

    private int FbytesTotal;

    public int bytesTotal
    {
      get { return FbytesTotal; }
      set { FbytesTotal = value; }
    }

    public WebExceptionStatus ErrorStatus;
    public HttpStatusCode StatusCode;
    public string StatusDesc;

    public bool UseMemoryStream = true;
    public MemoryStream memStream;

    #endregion
    protected System.ComponentModel.BackgroundWorker FWorker = null;
    public virtual void Start(System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
    {
      FWorker = worker;
      try
      {
        Execute();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    // Dictionary to store params for HTTP POST as 
    // (name) = the name of the parameter
    // (value) = the value for that parameter
    private Dictionary<string, string> FPostParams = new Dictionary<string, string>();
    private Dictionary<string, string> FHeaders = new Dictionary<string, string>();

    private bool FAlreadyDownloaded = false;

    // Default extension for the Temporary file
    private const string tmpExtension = ".tmp";

    // Constructor
    public FileDownload()
    {
    }

    public void AddParam(string name, string val)
    {
      FPostParams.Add(name, val);
    }

    private string GetParams()
    {
      string retVal = "";
      foreach (KeyValuePair<string, string> kvp in FPostParams)
      {
        retVal += kvp.Key + "=" + kvp.Value + "&";
      }
      return retVal.TrimEnd(new char[] { '&' });
    }

    public void AddHeader(string name, string val)
    {
      FHeaders.Add(name, val);
    }

    
    private bool IsCancelled()
    {
      if ((FWorker != null) && (FWorker.CancellationPending))
      {
        return true;
      }
      else
        return false; // change to member variable if necessary
    }

    private void ReportProgress(int downloadedBytes, int totalBytes)
    {
      this.bytesDone = downloadedBytes;
      this.bytesTotal = totalBytes;

      if (FWorker != null)
      {
        if (totalBytes != 0)
          FWorker.ReportProgress(Convert.ToInt32(downloadedBytes * 100 / totalBytes), this);
      }
    }

    public void DoDownload()
    {
      try
      {
        HttpWebRequest httpWebRequest = null;

        SetupRequest(out httpWebRequest);

        SaveResponse(httpWebRequest);


      }
      catch (Exception)
      {
        //throw exp;
      }
    }

    private void SaveResponse(HttpWebRequest httpWebRequest)
    {
      HttpWebResponse httpWebResponse = null;
      Stream responseStream = null;
      memStream = null;

      FAlreadyDownloaded = false;
      try
      {
        // get the response
        try
        {
          httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
          StatusCode = httpWebResponse.StatusCode;
          StatusDesc = httpWebResponse.StatusDescription;
        }
        catch (WebException exp)
        {
          ErrorStatus = exp.Status;

          if (exp.Status == WebExceptionStatus.ProtocolError)
          {
            StatusCode = ((HttpWebResponse)exp.Response).StatusCode;
            StatusDesc = ((HttpWebResponse)exp.Response).StatusDescription;

          }

          if (StatusCode == HttpStatusCode.NotModified)
            FAlreadyDownloaded = true;
          else
          {
            throw exp;
          }
        }

        if (!FAlreadyDownloaded)
        {
          responseStream = GetStreamForResponse(httpWebResponse);

          memStream = new MemoryStream();
          byte[] bytesRead = new byte[2048 * 5];

          int nCount;
          int nBytesRead = 0;
          int nBytesTotal = (int)httpWebRequest.ContentLength;

          while ((nCount = (int)(responseStream.Read(bytesRead, 0, bytesRead.Length))) > 0)
          {

            if (FWorker != null)
              if (FWorker.CancellationPending)
              {
                break;
              }

            memStream.Write(bytesRead, 0, nCount);
            nBytesRead += nCount;

            if (FWorker != null)
            {
              ReportProgress(nBytesRead, nBytesTotal);
            }

          } // end while

          if (!UseMemoryStream)
            CopyMemStreamToDestination();

          memStream.Position = 0;
        }
      } // end try
      finally
      {
        if (responseStream != null)
          responseStream.Close();
      }
    }
    private Stream GetStreamForResponse(HttpWebResponse httpWebResponse)
    {
      Stream stream;
      switch (httpWebResponse.ContentEncoding.ToUpperInvariant())
      {
        case "GZIP":
          stream = new GZipStream(httpWebResponse.GetResponseStream(), CompressionMode.Decompress);
          break;
        case "DEFLATE":
          stream = new DeflateStream(httpWebResponse.GetResponseStream(), CompressionMode.Decompress);
          break;

        default:
          stream = httpWebResponse.GetResponseStream();
          break;
      }
      return stream;
    }
    private bool ShouldUseCache()
    {
      if (FCheckCacheModified && (File.Exists(FDestinationPath)))
        return true;
      else
        return false;
    }

    private void SetupRequest(out HttpWebRequest httpWebRequest)
    {
      try
      {
        Uri uri = new Uri(FSourceURL);
        httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
        httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12 ( .NET CLR 3.5.30729; .NET4.0E)";
        httpWebRequest.KeepAlive = false;
        httpWebRequest.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        if (FAddReferrer)
          httpWebRequest.Referer = Referrer;

        foreach (KeyValuePair<string, string> kvp in FHeaders)
        {
          httpWebRequest.Headers.Add(kvp.Key, kvp.Value);
        }


        if (FDownloadMethod == DownloadMethod.Post)
        {
          Stream os = null;
          httpWebRequest.Method = "POST";


          if (FContentType == null)
            throw new Exception("ContentType not set for POST download");
          else
            httpWebRequest.ContentType = FContentType;

          ASCIIEncoding encoding = new ASCIIEncoding();
          byte[] bytes = encoding.GetBytes(GetParams());

          // send the Post
          httpWebRequest.ContentLength = bytes.Length;   //Count bytes to send
          try
          {
            os = httpWebRequest.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);         //Send it
          }
          finally
          {
            if (os != null)
              os.Close();
          }
        }
        else
        {
          if (ShouldUseCache())
            httpWebRequest.IfModifiedSince = FCacheLastModified;
          httpWebRequest.Method = "GET";
        }
      }
      finally
      {
      }

    }

    private void CopyMemStreamToDestination()
    {
      // File downloaded - Now rename it, if Download NOT cancelled
      if (FWorker == null || (FWorker != null && !FWorker.CancellationPending))
      {
        // Delete destination file if it exists
        if (File.Exists(FDestinationPath))
          File.Delete(FDestinationPath);

        // Copy temporary file if it exists, and then delete it

        FileStream fs = File.OpenWrite(FDestinationPath);
        memStream.Position = 0;
        try
        {
          byte[] buffer = new byte[32768];
          while (true)
          {
            int read = memStream.Read(buffer, 0, buffer.Length);
            if (read <= 0)
              return;
            fs.Write(buffer, 0, read);
          }
        }
        finally
        {
          fs.Close();
        }
      } // end Renaming section
    }


    // Destructor
    ~FileDownload()
    {

    } // end destructor FileDownload()


    #region IThreadable Members


    public bool IsAsync()
    {
      return FIsAsyncDownload;
    }

    public void Execute()
    {
      DoDownload();
    }

    #endregion
  } // end class
} // end namespace
