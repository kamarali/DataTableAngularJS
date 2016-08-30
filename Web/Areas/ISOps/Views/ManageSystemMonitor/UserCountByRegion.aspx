<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<head runat="server">
  <title>Google Visualization API Sample</title>
     <script type="text/javascript" src="http://www.google.com/jsapi"></script>
  <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.5.1/jquery.min.js"></script>
  <script type="text/javascript">

    // load visualization library
    google.load('visualization', '1', { packages: ['geomap'] });

    $(function () {
      // when document loads, grab the json
      $.ajax({
        type: "POST",
        url: '<%:Url.Action("GetUserCountByRegion", "ManageSystemMonitor", new { area = "ISOps"}) %>',
        dataType: "json",
        success: function (response) {
          var map = new google.visualization.DataTable();
          map.addRows(response.length);  // length gives us the number of results in our returned data
          map.addColumn('string', 'Country');
          map.addColumn('number', 'Logged-in User Count');

          if (response.length == 0) {
            map.addRows(1);
          }


          $.each(response, function (i, v) {
            //set the values for both the name and the population
            map.setValue(i, 0, v.CountryName);
            map.setValue(i, 1, v.UserCount);

          });

          var geomap = new google.visualization.GeoMap(document.getElementById('visualization'));
          geomap.draw(map, null);
        },
        failure: function (response) {

        }
      });



    });
  </script>

</head>
<body>
<div id="visualization"></div>
</body>
</html>
