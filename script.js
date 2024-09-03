async function loadRoomsWithDevices() {
  try {
    const response = await fetch(
      "https://smarthomeb.mercantec.tech/api/Rooms/WithDevices"
    );
    if (!response.ok) {
      throw new Error("Network response was not ok");
    }
    const roomsWithDevices = await response.json();
    const house = document.getElementById("house");

    // Ryd huset, før du tilføjer nye værelser
    house.innerHTML = "";

    roomsWithDevices.forEach((room) => {
      const roomDiv = document.createElement("div");
      roomDiv.className = "room";
      roomDiv.innerHTML = `<strong>${room.roomName || "Ukendt rum"}</strong>`;

      if (room.devices.length > 0) {
        room.devices.forEach((device) => {
          const deviceDiv = document.createElement("div");
          deviceDiv.className = "device";

          // Tilføj passende klasse baseret på enhedstype
          switch (device.type) {
            case "Lys":
              deviceDiv.classList.add("light");
              deviceDiv.classList.add(device.isOn ? "on" : "off");
              break;
            case "Tv":
              deviceDiv.classList.add("tv");
              deviceDiv.classList.add(device.isOn ? "on" : "off");
              break;
            case "Radio":
              deviceDiv.classList.add("radio");
              deviceDiv.classList.add(device.isOn ? "on" : "off");
              break;
            default:
              deviceDiv.classList.add("unknown");
              deviceDiv.textContent = "Ukendt enhed";
              return;
          }

          deviceDiv.textContent = device.name || "Ukendt enhed";
          roomDiv.appendChild(deviceDiv);
        });
      }

      house.appendChild(roomDiv);
    });
  } catch (error) {
    console.error("Der opstod en fejl:", error);
  }
}

window.onload = function () {
  loadRoomsWithDevices();

  // WebSocket-forbindelse
  const socket = new WebSocket("wss://smarthomeb.mercantec.tech/ws/updates");

  socket.addEventListener("message", function (event) {
    // Opdater visningen, når en besked modtages
    loadRoomsWithDevices();
  });

  socket.addEventListener("error", function (event) {
    console.error("WebSocket error:", event);
  });
};
