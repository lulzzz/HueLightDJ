using HueLightDJ.Effects;
using HueLightDJ.Web.Models;
using Q42.HueApi.Streaming.Effects;
using Q42.HueApi.Streaming.Extensions;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HueLightDJ.Web.Streaming
{
  public static class GroupService
  {
    public static List<GroupModel> GetAll(EntertainmentLayer layer = null)
    {
      if (layer == null)
        layer = StreamingSetup.Layers.First();

      var center = EffectSettings.LocationCenter;

      var allLightsOrdered = layer.OrderBy(x => x.LightLocation.X).ThenBy(x => x.LightLocation.Y).ToList();

      var leftRight = new[] { allLightsOrdered.GetLeft(), allLightsOrdered.GetRight() }.ToList();
      var frontBack = new[] { allLightsOrdered.GetFront(), allLightsOrdered.GetBack() }.ToList();
      var quarter = new[] { layer.GetLeft().GetFront(), layer.GetLeft().GetBack(), layer.GetRight().GetBack(), layer.GetRight().GetFront() }.ToList();
      var alternating = allLightsOrdered.ChunkByGroupNumber(2);
      var alternatingFour = allLightsOrdered.ChunkByGroupNumber(4);
      var orderedByDistance = layer.OrderBy(x => x.LightLocation.Distance(center.X, center.Y)).ChunkByGroupNumber(3);
      var orderedByAngle = layer.OrderBy(x => x.LightLocation.Angle(center.X, center.Y)).ChunkBy(6);

      var leftToRight = allLightsOrdered.GroupBy(x => (int)(((x.LightLocation.X + 1) / 2) * 50));
      var fronToBack = allLightsOrdered.GroupBy(x => (int)(((x.LightLocation.Y + 1) / 2) * 50));
      var ring = allLightsOrdered.GroupBy(x => (int)((x.LightLocation.Distance(center.X, center.Y) / 1.5) * 2));

      var result = new List<GroupModel>()
      {
        //new GroupModel("All", allLightsOrdered.To2DGroup()),
        new GroupModel("Left/Right", leftRight),
        new GroupModel("Left To Right", leftToRight, 2),
        new GroupModel("Front/Back", frontBack),
        new GroupModel("Front To Back", fronToBack, 2),
        new GroupModel("Quarter", quarter, 2),
        new GroupModel("Alternating", alternating),
        new GroupModel("Alternating by 4", alternatingFour),
        //new GroupModel("Distance from center", orderedByDistance),
        //new GroupModel("Order by Angle from center", orderedByAngle),
        new GroupModel("Ring", ring, 2),
        new GroupModel("Random", GetRandomGroup()),
      };

      if (StreamingSetup.CurrentConnection.Name == "Bar")
      {
        //TODO: Add groups for Bar location
      }
     

      return result;
    }

    public static IEnumerable<IEnumerable<EntertainmentLight>> GetRandomGroup()
    {
      var layer = StreamingSetup.Layers.First();
      var orderRandom = layer.OrderBy(x => Guid.NewGuid());

      var min = 2;
      var max = layer.Count / 2;

      if (max < min)
        max = min;

      Random r = new Random();
      var numberOfGroups =  r.Next(min, max);

      return orderRandom.ChunkBy(numberOfGroups);

    }
  }
}
