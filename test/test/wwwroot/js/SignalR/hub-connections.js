// create connections
const mailConnection = new signalR.HubConnectionBuilder()
  .withUrl("/mailHub")
  .withAutomaticReconnect()
  .build();

const userModuleConnection = new signalR.HubConnectionBuilder()
  .withUrl("/userModuleHub")
  .withAutomaticReconnect()
  .build();

// start connections
async function startConnections() {
  try {
    await Promise.all([mailConnection.start(), userModuleConnection.start()]);
    console.log("Connected to SignalR hubs");
  } catch (err) {
    console.error("Error connecting to hubs:", err);
    // try to connect again after 5 seconds
    setTimeout(startConnections, 5000);
  }
}

// Mail hub handlers
mailConnection.on("ReceiveMailNotification", (message, type) => {
  console.log("Received mail notification:", message, type);
  switch (type) {
    case "create":
    case "update":
    case "delete":
      if (window.location.pathname === "/Mails") {
        if (typeof loadData === "function") {
          loadData();
        }
      }
      break;
  }
});

// User module hub handlers
userModuleConnection.on("ReceiveUserModuleNotification", (message, type) => {
  console.log("Received user module notification:", message, type);
  switch (type) {
    case "create":
    case "update":
    case "delete":
      window.location.reload();
      break;
  }
});

// handle when connections are lost
mailConnection.onclose(async () => {
  await startConnections();
});

userModuleConnection.onclose(async () => {
  await startConnections();
});

// start connections when page loads
startConnections();
