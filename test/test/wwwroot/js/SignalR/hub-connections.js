const token = localStorage.getItem("token");
const payload = JSON.parse(atob(token.split(".")[1]));
const userId = payload.id;

let mailConnection;
let userModuleConnection;
let userOnlineConnection;
if (token) {
  mailConnection = new signalR.HubConnectionBuilder()
    .withUrl("/mailHub")
    .withAutomaticReconnect()
    .build();

  userModuleConnection = new signalR.HubConnectionBuilder()
    .withUrl("/userModuleHub")
    .withAutomaticReconnect()
    .build();

  userOnlineConnection = new signalR.HubConnectionBuilder()
    .withUrl("/userOnlineHub", {
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .build();

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
        sessionStorage.removeItem("userModules");
        checkPermissions();
        window.location.reload();
        break;
    }
  });

  // User online hub handlers
  userOnlineConnection.on("GetUsersCount", () => {
    if (window.location.pathname === "/Statistics") {
      if (typeof getUserCount === "function") {
        getUserCount();
      }
    }
  });

  userOnlineConnection.on("GetUserAccessTime", () => {
    if (window.location.pathname === "/Statistics") {
      if (typeof getUserAccessTimes === "function") {
        getUserAccessTimes();
      }
    }
  });

  mailConnection.onclose(async () => {
    await startConnections();
  });

  userModuleConnection.onclose(async () => {
    await startConnections();
  });

  userOnlineConnection.onclose(async () => {
    await startConnections();
  });

  // start connections when page loads
  startConnections();
}

// start connections
async function startConnections() {
  if (!token) return;
  try {
    await Promise.all([
      mailConnection.start(),
      userModuleConnection.start(),
      userOnlineConnection.start(),
    ]);

    // if (mailConnection.state === signalR.HubConnectionState.Disconnected) {
    //   await mailConnection.start();
    // }
    // if (
    //   userModuleConnection.state === signalR.HubConnectionState.Disconnected
    // ) {
    //   await userModuleConnection.start();
    // }
    // if (
    //   userOnlineConnection.state === signalR.HubConnectionState.Disconnected
    // ) {
    //   await userOnlineConnection.start();
    // }

    console.log("Connected to SignalR hubs");
  } catch (err) {
    console.error("Error connecting to hubs:", err);
    // try to connect again after 5 seconds
    setTimeout(startConnections, 5000);
  }
}
