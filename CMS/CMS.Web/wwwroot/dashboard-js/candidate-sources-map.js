const mapContainer = document.querySelector('#candidate-sources-map');
const locations = JSON.parse(mapContainer.dataset.locations.replaceAll("'",'"')) ?? [];

const markers = locations.map(l=>{
    const coords = countries[l];
    return { name: l, coords: coords}
})


const  worldemapmarkers = new jsVectorMap({
    map: "world_merc",
    selector: "#candidate-sources-map",
    zoomOnScroll: true,
    zoomButtons: false,
    selectedMarkers: [],
    regionStyle: {
      initial: {
        stroke: "#9599ad",
        strokeWidth: 0.25,
        fill: "#f3f6f9",
        fillOpacity: 1,
      },
    },
    markersSelectable: false,
    markers: markers,
    markerStyle: { initial: { fill: "#0ab39c" }, selected: { fill: "#405189" } },
    labels: {
      markers: {
        render: function (e) {
          return e.name;
        },
      },
    },
  });
