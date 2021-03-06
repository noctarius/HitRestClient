﻿using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;



namespace HIT.REST.Client.Hit {

  /// <summary>
  /// Beschreibt vollständige URI, deren Komponenten sich frei
  /// ändern lassen. Lediglich die unsicheren Komponenten "User" und "Password" fehlen.
  /// Man muss wissen, was man tut, es werden kaum semantische
  /// Prüfungen vorgenommen!
  /// </summary>
  public class URI  {
//--------------------------------------------------------------------

    public String Scheme    {
      get { return strThisScheme; }
      set {
        if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException();
        strThisScheme = value.ToLower();
      }
    }
    private String strThisScheme;

    public String Host      { get; set; }

    public int?   Port      { get; set; }

    /// <summary>
    /// Basis-Pfad, der für alle REST-Anfragen identisch ist.
    /// </summary>
    public String BasePath  {
      get { return strThisBasePath; }
      set {
        if (value != null)  {
          while (value.StartsWith("/"))  value = value.Substring(1);
          while (value.EndsWith("/"))    value = value.Substring(0,value.Length-1);
        }
        strThisBasePath = value;
      }
    }
    private String strThisBasePath;

    public String RestPath  { get; set; }

    public NameValueCollection  Query { get; private set; }  // no public set!

    public String Fragment  { get; set; }



//--------------------------------------------------------------------

    public URI()  {
      Scheme  = "http";
      Query   = new NameValueCollection();
    }


    /// <summary>
    /// Lege Kopie einer URI an.
    /// </summary>
    /// <param name="pobjSource"></param>
    public URI(URI pobjSource)  {
      if (pobjSource == null) throw new ArgumentNullException();

      strThisScheme   = pobjSource.strThisScheme;
      Host            = pobjSource.Host;
      Port            = pobjSource.Port;
      BasePath        = pobjSource.strThisBasePath;
      RestPath        = pobjSource.RestPath;
      Query           = new NameValueCollection(pobjSource.Query);      // copy
      Fragment        = pobjSource.Fragment;
    }


    /// <summary>
    /// Liefere Kombination aus BasePath und RestPath.
    /// </summary>
    public String FullPath {
      get {
        if (String.IsNullOrEmpty(BasePath)) {
          if (String.IsNullOrEmpty(RestPath)) {
            return "";
          }
          else  {
            return RestPath;
          }
        }
        else  {
          if (String.IsNullOrEmpty(RestPath)) {
            return BasePath;
          }
          else  {
            return BasePath+"/"+RestPath;
          }
        }
      }
    }



//--------------------------------------------------------------------

    /// <summary>
    /// Erzeuge vollständige URI.
    /// Ist kein Host angegeben, wird eine relative URI gebildet.
    /// </summary>
    /// <returns>URI oder <tt>null</tt> bei grobem Fehler</returns>
    public override string ToString() {
      StringBuilder strBuf = new StringBuilder();

      // Ist ein Host angegeben, dann mit absolutem PFad beginnen
      if (!String.IsNullOrWhiteSpace(Host)) {
        // "scheme://host"
        strBuf.Append(Scheme).Append("://").Append(Host);
        // ":port"
        if (Port != null && Port > 0) {
          bool addPort = false;
          addPort |= (Port !=  80 && "http" .Equals(Scheme));
          addPort |= (Port != 443 && "https".Equals(Scheme));
          if (addPort) strBuf.Append(":").Append(Port);
        }
        // "/"
        strBuf.Append("/");
      }

      // "path"
      // zum Urlencoden zerlegen
      String[] astrFolders = FullPath.Split('/');
      bool first = true;
      foreach (String folder in astrFolders) {
        if (first) first=false; else strBuf.Append("/");
        strBuf.Append(HttpUtility.UrlEncode(folder));
      }

      // "?query"
      if (Query.Count > 0)  {
        strBuf.Append("?");
        first = true;
        foreach (String key in Query) {
          String val = Query[key];
          if (first) first=false; else strBuf.Append("&");
          strBuf.Append(HttpUtility.UrlEncode(key)).Append("=").Append(HttpUtility.UrlEncode(val));
        }
      }

      // "#fragment"
      if (!String.IsNullOrWhiteSpace(Fragment))  {
        strBuf.Append("#").Append(Fragment);
      }

      return strBuf.ToString();
    }



//--------------------------------------------------------------------
  }
}
