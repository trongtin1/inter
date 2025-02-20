// create connection
const connection = new signalR.HubConnectionBuilder()
  .withUrl("/userModuleHub")
  .withAutomaticReconnect()
  .build();

// start connection
async function startConnection() {
  try {
    await connection.start();

    console.log("Connected to SignalR hub");

    // get userId from JWT token
    // const token = sessionStorage.getItem("token");
    // if (token) {
    //   const payload = JSON.parse(atob(token.split(".")[1]));
    //   // Join to group with userId
    //   console.log("user_" + payload.nameid);
    //   await connection.invoke("JoinGroup", "user_" + payload.nameid);

    // }
  } catch (err) {
    console.error("Error connecting to hub:", err);
    // try to connect again after 5 seconds
    setTimeout(startConnection, 5000);
  }
}

// handle notification event
connection.on("ReceiveUserModuleNotification", (message, type) => {
  console.log("Received notification:", message, type);

  // update UI depending on notification type
  switch (type) {
    case "create":
    case "update":
    case "delete":
      // Tải lại trang web
      window.location.reload();
      break;
  }
});

// handle when connection is lost
connection.onclose(async () => {
  await startConnection();
});

// start connection when page load
startConnection();
