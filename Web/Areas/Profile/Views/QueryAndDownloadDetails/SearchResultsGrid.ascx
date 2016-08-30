<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<style type="text/css">
  .tablewrap {
    overflow: auto;
  }
   .ui-jqgrid .loading {position: absolute; top: 45%;left: 1%;width: auto;z-index:101;padding: 6px; margin: 5px;text-align: center;font-weight: bold;display: none;border-width: 2px !important;}
</style>
<script type="text/javascript">
  $(function () {
      var version = $.browser.msie && $.browser.version.substr(0, 1);
    if (version <= 7) {
      $('#gridDiv').removeClass('tablewrap');
    }
    else {
      $('#gridDiv').addClass('tablewrap');
    }


//    // distributed by http://www,hypergurl.com  
//    var debug = true;
//    function right(e) {
//      if (navigator.appName == 'Netscape' && (e.which == 3 || e.which == 2))
//        return false;
//      else if (navigator.appName == 'Microsoft Internet Explorer' && (event.button == 2 || event.button == 3)) {
//        alert('This Page is fully protected!');
//        return false;
//      }
//      return true;
//    }
//    document.onmousedown = right;
//    if (document.layers)
//      window.captureEvents(Event.MOUSEDOWN);
//    window.onmousedown = right;

  });
</script>
<div id="divSearchResult">
  <div id="gridDiv">
    <table id="list">
    </table>
  </div>
  <div id="pager">
  </div>
  <div class="buttonContainer">
    <input type="submit" class="primaryButton" value="Download" id="btnDownloadCSV" onclick="return downloadReport();" />
    <input class="secondaryButton" type="button" value="Exit" onclick="$Resultdialog.dialog('close');" />
  </div>
</div>
<div id="addressLabel">
  <div id="addressLabelFormat" style="height: 480px; overflow: auto;">
  </div>
  <div class="buttonContainer">
    <input type="submit" class="primaryButton" value="Download" id="btnDownloadPDF" onclick="return downloadReport();" />
    <input class="secondaryButton" type="button" value="Exit" onclick="$AddressLabelResultdialog.dialog('close');" />
  </div>
</div>
