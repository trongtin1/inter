const token = localStorage.getItem("token");
const payload = JSON.parse(atob(token.split(".")[1]));
const userId = payload.id;
let userOnlineConnection;
if (token) {
  // create connection
  userOnlineConnection = new signalR.HubConnectionBuilder()
    .withUrl("/userOnlineHub", {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .build();

  userOnlineConnection.on("GetUserAccessTime", () => {
    if (window.location.pathname === "/Statistics") {
      if (typeof getUserAccessTimes === "function") {
        getUserAccessTimes();
      }
    }
  });

  // handle when connection is lost
  userOnlineConnection.onclose(async () => {
    await startConnections();
  });

  // start connection when page load
  startConnection();
}
async function startConnections() {
  if (!token) return;
  try {
    await Promise.all([userOnlineConnection.start()]);
    console.log("Connected to SignalR hubs");
  } catch (err) {
    console.error("Error connecting to hubs:", err);
    // try to connect again after 5 seconds
    setTimeout(startConnections, 5000);
  }
}
