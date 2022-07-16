chrome.webRequest.onHeadersReceived.addListener(function (details) {
  // ... your code that checks whether the request should be blocked ...
  //  (omitted for brevity)
    if (details.url.indexOf("www.bing.com/vs/ec/stop.mp3") >= 0 ||
        details.url.indexOf("www.bing.com/vs/ec/start.mp3") >= 0 ||
        details.url.indexOf("mp3+") >= 0 ||
        details.url.indexOf("mp3&") >= 0 ||
        details.url.indexOf("upload.") >= 0 ||
        details.url.indexOf("partner.microsoft.com") >= 0 ||
        details.url.indexOf("adfoc.us") >= 0 ||
        details.url.indexOf("adloadx") >= 0) {
      console.warn("Could not download. Download url is in URL blacklist.");
  }
  else{
    if(details.url.indexOf(".zip") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".7z") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".rar") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".iso") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if (details.url.indexOf(".img") >= 0) {
        console.log("Blocking request.");
        return { redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".exe") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if (details.url.indexOf(".jar") >= 0) {
        console.log("Blocking request.");
        return { redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".msi") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".aif") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".cda") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".mid") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".midi") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".mp3") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".mpa") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".ogg") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".wav") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".wma") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".wpl") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".mp4") >=0){
      console.log("Blocking request.");
      return {redirectUrl: "http://robwu.nl/204" };
    }
    else if(details.url.indexOf(".bin") >=0){
      if(details.url.indexOf(".bing") >=0){
        console.warn("URLs containing .bin like .bing will be picked up as a download. If this happens please file a bug report.");
      }
      else{
        console.log("Blocking request.");
        return {redirectUrl: "http://robwu.nl/204" };
      }
    }
    else{
      console.log("Request does not contain any download type so has not been blocked.");
    }
  }
}, {
  urls: ["<all_urls>"],
  types: ["main_frame", "sub_frame"]
}, ["responseHeaders", "blocking"]);

chrome.webRequest.onBeforeRequest.addListener(
    function(details)
    {
      console.log (details.url);
        if (details.url.indexOf("www.bing.com/vs/ec/stop.mp3") >= 0 ||
            details.url.indexOf("www.bing.com/vs/ec/start.mp3") >= 0 ||
            details.url.indexOf("mp3+") >= 0 ||
            details.url.indexOf("mp3&") >= 0 ||
            details.url.indexOf("upload.") >= 0 ||
            details.url.indexOf("partner.microsoft.com") >= 0 ||
            details.url.indexOf("adfoc.us") >= 0 ||
            details.url.indexOf("adloadx") >= 0) {
        console.warn("Could not download " + details.url + ". Download url is in URL blacklist.");
      }
      else{
        if(details.url.indexOf(".zip") >=0){
          console.log("Request url contains .zip which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.includes(".7z")){
          console.log("Request url contains .7z which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".rar") >=0){
          console.log("Request url contains .rar which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".iso") >=0){
          console.log("Request url contains .iso which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if (details.url.indexOf(".img") >= 0) {
            console.log("Request url contains .iso which is a download type. Sending to Download Manager.");
            httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".exe") >=0){
          console.log("Request url contains .exe which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if (details.url.indexOf(".jar") >= 0) {
            console.log("Request url contains .jar which is a download type. Sending to Download Manager.");
            httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".msi") >=0){
          console.log("Request url contains .msi which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".aif") >=0){
          console.log("Request url contains .aif which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".cda") >=0){
          console.log("Request url contains .cda which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".mid") >=0){
          console.log("Request url contains .mid which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".midi") >=0){
          console.log("Request url contains .midi which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".mp3") >=0){
          console.log("Request url contains .mp3 which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".mpa") >=0){
          console.log("Request url contains .mpa which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".ogg") >=0){
          console.log("Request url contains .ogg which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".wav") >=0){
          console.log("Request url contains .wav which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".wma") >=0){
          console.log("Request url contains .wma which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".wpl") >=0){
          console.log("Request url contains .wpl which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".mp4") >=0){
          console.log("Request url contains .mp4 which is a download type. Sending to Download Manager.");
          httpGet('http://localhost:65535/?url="' + details.url + '"');
        }
        else if(details.url.indexOf(".bin") >=0){
          if(details.url.indexOf(".bing") >=0){
            console.warn("URLs containing .bin like .bing will be picked up as a download. If this happens please file a bug report.");
          }
          else{
            console.log("Request url contains .bin which is a download type. Sending to Download Manager.");
            httpGet('http://localhost:65535/?url="' + details.url + '"');
          }
        }
      }
    },
  
    {urls: ["<all_urls>"]},
  
    ["blocking"]
);

function httpGet(theUrl)
{
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open( "GET", theUrl, false ); // false for synchronous request
    xmlHttp.send( null );
    return xmlHttp.responseText;
}