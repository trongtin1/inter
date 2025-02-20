let allUserData = [];

function getUserCount() {
  axios
    .get("/api/UserOnline/users-count")
    .then((response) => {
      updateUserCount(response.data.data);
    })
    .catch((error) => {
      console.error("Error fetching user count:", error);
    });
}

function getUserAccessTimes() {
  axios
    .get("/api/UserOnline/users-access-times")
    .then((response) => {
      allUserData = response.data.data;
      initializeSelect2();
      updateUserAccessTime(allUserData);
    })
    .catch((error) => {
      console.error("Error fetching user access times:", error);
    });
}

function initializeSelect2() {
  const userOptions = allUserData.map((user) => ({
    id: user.name,
    text: `${user.name} (${user.email})`,
  }));

  $("#nameSearch").select2({
    width: "180px",
    data: [{ id: "", text: "Choose User" }, ...userOptions],
    placeholder: "Choose User",
    allowClear: true,
  });
}

function updateUserCount(count) {
  $("#currentUserCount").text(count);
}

function updateUserAccessTime(accessTimes) {
  const $tableBody = $("#userAccessTableBody");
  if (!$tableBody.length) return;

  const nameFilter = $("#nameSearch").val();
  const statusFilter = $("#statusFilter").val();

  const filteredData = accessTimes.filter((user) => {
    const nameMatch = !nameFilter || user.name === nameFilter;
    const statusMatch =
      statusFilter === "all" ||
      (statusFilter === "online" && user.isOnline) ||
      (statusFilter === "offline" && !user.isOnline);

    return nameMatch && statusMatch;
  });
  $tableBody.empty();

  $.each(filteredData, function (index, user) {
    $tableBody.append(`
      <tr>
        <td>${index + 1}</td>
        <td>${user.name}</td>
        <td>${user.email}</td>
        <td>${user.connectedTime}</td>
        <td>${user.totalConnectedTime}</td>
        <td>${user.isOnline ? "Online" : "Offline"}</td>
      </tr>
    `);
  });
}

$(function () {
  getUserCount();
  getUserAccessTimes();

  $("#nameSearch").on("change", function () {
    updateUserAccessTime(allUserData);
  });

  $("#statusFilter").on("change", function () {
    updateUserAccessTime(allUserData);
  });
});
