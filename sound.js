async function loadSoundUnits() {
  try {
    const response = await fetch(
      "https://smarthomeb.mercantec.tech/api/SoundUnits"
    );
    if (!response.ok) {
      throw new Error("Network response was not ok");
    }
    const soundUnits = await response.json();
    const soundUnitsContainer = document.getElementById("sound-units");

    // Clear the container before adding new sound units
    soundUnitsContainer.innerHTML = "";

    soundUnits.forEach((unit) => {
      const unitDiv = document.createElement("div");
      unitDiv.className = "sound-unit";
      unitDiv.innerHTML = `<strong>${unit.unitName || "Ukendt enhed"}</strong>`;

      const chartsDiv = document.createElement("div");
      chartsDiv.className = "charts";

      const canvas1 = document.createElement("canvas");
      canvas1.id = `chart1-${unit.id}`;
      chartsDiv.appendChild(canvas1);

      const canvas2 = document.createElement("canvas");
      canvas2.id = `chart2-${unit.id}`;
      chartsDiv.appendChild(canvas2);

      unitDiv.appendChild(chartsDiv);
      soundUnitsContainer.appendChild(unitDiv);

      if (unit.decibelData.length > 0) {
        const labels = unit.decibelData.map((data) => new Date(data.time));
        const data = unit.decibelData.map((data) => data.decibel);

        const ctx1 = document
          .getElementById(`chart1-${unit.id}`)
          .getContext("2d");
        new Chart(ctx1, {
          type: "line",
          data: {
            labels: labels,
            datasets: [
              {
                label: "Decibel",
                data: data,
                borderColor: "rgba(75, 192, 192, 1)",
                borderWidth: 1,
                fill: false,
              },
            ],
          },
          options: {
            scales: {
              x: {
                type: "time",
                time: {
                  unit: "minute",
                },
                title: {
                  display: true,
                  text: "Time",
                },
              },
              y: {
                title: {
                  display: true,
                  text: "Decibel",
                },
              },
            },
          },
        });

        const ctx2 = document
          .getElementById(`chart2-${unit.id}`)
          .getContext("2d");
        new Chart(ctx2, {
          type: "bar",
          data: {
            labels: labels,
            datasets: [
              {
                label: "Decibel",
                data: data,
                backgroundColor: "rgba(75, 192, 192, 0.5)",
                borderColor: "rgba(75, 192, 192, 1)",
                borderWidth: 1,
              },
            ],
          },
          options: {
            scales: {
              x: {
                type: "time",
                time: {
                  unit: "minute",
                },
                title: {
                  display: true,
                  text: "Time",
                },
              },
              y: {
                title: {
                  display: true,
                  text: "Decibel",
                },
              },
            },
          },
        });
      } else {
        unitDiv.innerHTML += "<p>No decibel data available</p>";
      }
    });
  } catch (error) {
    console.error("Der opstod en fejl:", error);
  }
}

window.onload = function () {
  loadSoundUnits();

  // WebSocket-forbindelse
  const socket = new WebSocket("wss://smarthomeb.mercantec.tech/ws/updates");

  socket.addEventListener("message", function (event) {
    // Opdater visningen, når en besked modtages
    loadSoundUnits();
  });

  socket.addEventListener("error", function (event) {
    console.error("WebSocket error:", event);
  });
};
