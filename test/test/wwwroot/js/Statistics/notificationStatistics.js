let monthlyNotiChart;
let notiFrequencyChart;
let monthlyUserNotiChart;
let notiTypeDistributionChart;
let userNotiStatsChart;
let userNotiTypeStatsChart;

async function loadNotiFrequencies() {
  try {
    const response = await axios.get(
      "/api/NotiStatistics/most-noti-frequencies",
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    if (response.data.success) {
      const emails = response.data.data.map((item) => item.email);
      const counts = response.data.data.map((item) => item.count);

      if (notiFrequencyChart) {
        notiFrequencyChart.destroy();
      }

      const ctx = $("#notiFrequencyChart")[0].getContext("2d");
      notiFrequencyChart = new Chart(ctx, {
        type: "bar",
        data: {
          labels: emails,
          datasets: [
            {
              label: "Number of Notifications",
              data: counts,
              backgroundColor: "rgba(255, 99, 132, 0.5)",
              borderColor: "rgba(255, 99, 132, 1)",
              borderWidth: 1,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: true,
          scales: {
            y: {
              beginAtZero: true,
              ticks: {
                stepSize: 1,
              },
            },
            x: {
              ticks: {
                maxRotation: 45,
                minRotation: 45,
              },
            },
          },
          plugins: {
            legend: {
              display: true,
              position: "top",
            },
            title: {
              display: true,
              text: "Most Frequent Notification Recipients",
              padding: {
                top: 10,
                bottom: 30,
              },
            },
          },
        },
      });
    }
  } catch (error) {
    console.error(
      "Error loading notification frequencies:",
      error.response?.data?.message || error.message
    );
  }
}

async function loadMonthlyNotiStats(year) {
  try {
    const response = await axios.get(
      `/api/NotiStatistics/noti-frequencies-by-year`,
      {
        params: { year: year },
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    if (response.data.success) {
      const monthlyStats = response.data.data.monthlyStats;
      const totalNotifications = monthlyStats.reduce(
        (sum, stat) => sum + stat.count,
        0
      );

      const monthNames = [
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec",
      ];
      const labels = monthNames;
      const counts = monthlyStats.map((stat) => stat.count);

      if (monthlyNotiChart) {
        monthlyNotiChart.destroy();
      }

      const ctx = $("#monthlyNotiChart")[0].getContext("2d");
      monthlyNotiChart = new Chart(ctx, {
        type: "bar",
        data: {
          labels: labels,
          datasets: [
            {
              label: "Number of Notifications",
              data: counts,
              backgroundColor: "rgba(75, 192, 192, 0.5)",
              borderColor: "rgba(75, 192, 192, 1)",
              borderWidth: 1,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: true,
          scales: {
            y: {
              beginAtZero: true,
              ticks: {
                stepSize: 1,
              },
            },
          },
          plugins: {
            legend: {
              display: true,
              position: "top",
            },
            title: {
              display: true,
              text: [
                `Monthly Notification Statistics - ${year}`,
                `Total Notifications: ${totalNotifications}`,
              ],
              padding: {
                top: 10,
                bottom: 30,
              },
            },
          },
        },
      });
    }
  } catch (error) {
    console.error(
      "Error loading monthly statistics:",
      error.response?.data?.message || error.message
    );
  }
}

async function loadUserNotiStats(email, year) {
  try {
    const response = await axios.get(`/api/NotiStatistics/noti-monthly-stats`, {
      params: {
        email: email,
        year: year,
      },
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    });

    if (response.data.success) {
      const monthlyStats = response.data.data.monthlyStats;
      const totalNotifications = response.data.data.yearlyTotal;

      const monthNames = [
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec",
      ];
      const labels = monthNames;
      const counts = monthlyStats.map((stat) => stat.count);

      if (monthlyUserNotiChart) {
        monthlyUserNotiChart.destroy();
      }

      const ctx = $("#monthlyUserNotiChart")[0].getContext("2d");
      monthlyUserNotiChart = new Chart(ctx, {
        type: "bar",
        data: {
          labels: labels,
          datasets: [
            {
              label: "Number of Notifications",
              data: counts,
              backgroundColor: "rgba(255, 159, 64, 0.5)",
              borderColor: "rgba(255, 159, 64, 1)",
              borderWidth: 1,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: true,
          scales: {
            y: {
              beginAtZero: true,
              ticks: {
                stepSize: 1,
              },
            },
          },
          plugins: {
            legend: {
              display: true,
              position: "top",
            },
            title: {
              display: true,
              text: [
                `Monthly Notification Statistics - ${email} (${year})`,
                `Total Notifications: ${totalNotifications}`,
              ],
              padding: {
                top: 10,
                bottom: 30,
              },
            },
          },
        },
      });
    }
  } catch (error) {
    console.error(
      "Error loading user statistics:",
      error.response?.data?.message || error.message
    );
  }
}

async function loadNotiTypeDistribution() {
  try {
    const response = await axios.get("/api/NotiStatistics/type-distribution", {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    });

    if (response.data.success) {
      const data = response.data.data;
      const types = data.map((item) => item.type);
      const counts = data.map((item) => item.count);
      const unreadCounts = data.map((item) => item.unreadCount);
      const unseenCounts = data.map((item) => item.unseenCount);

      if (notiTypeDistributionChart) {
        notiTypeDistributionChart.destroy();
      }

      const ctx = $("#notiTypeDistributionChart")[0].getContext("2d");
      notiTypeDistributionChart = new Chart(ctx, {
        type: "doughnut",
        data: {
          labels: types,
          datasets: [
            {
              data: counts,
              backgroundColor: [
                "rgba(255, 99, 132, 0.5)",
                "rgba(54, 162, 235, 0.5)",
                "rgba(255, 206, 86, 0.5)",
                "rgba(75, 192, 192, 0.5)",
                "rgba(153, 102, 255, 0.5)",
              ],
              borderColor: [
                "rgba(255, 99, 132, 1)",
                "rgba(54, 162, 235, 1)",
                "rgba(255, 206, 86, 1)",
                "rgba(75, 192, 192, 1)",
                "rgba(153, 102, 255, 1)",
              ],
              borderWidth: 1,
            },
          ],
        },
        options: {
          responsive: true,
          plugins: {
            legend: {
              position: "right",
            },
            title: {
              display: true,
              text: "Notification Type Distribution",
            },
          },
        },
      });
    }
  } catch (error) {
    console.error(
      "Error loading notification type distribution:",
      error.response?.data?.message || error.message
    );
  }
}

async function loadUserNotiOverview(email) {
  try {
    const response = await axios.get(
      `/api/NotiStatistics/user-stats-by-email`,
      {
        params: { email: email },
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    if (response.data.success) {
      const stats = response.data.data[0] || {
        totalNotifications: 0,
        readCount: 0,
        unreadCount: 0,
        seenCount: 0,
        unseenCount: 0,
      };

      if (userNotiStatsChart) {
        userNotiStatsChart.destroy();
      }

      const ctx = $("#userNotiStatsChart")[0].getContext("2d");
      userNotiStatsChart = new Chart(ctx, {
        type: "bar",
        data: {
          labels: ["Total", "Read", "Unread", "Seen", "Unseen"],
          datasets: [
            {
              axis: "y",
              data: [
                stats.totalNotifications || 0,
                stats.readCount || 0,
                stats.unreadCount || 0,
                stats.seenCount || 0,
                stats.unseenCount || 0,
              ],
              backgroundColor: [
                "rgba(75, 192, 192, 0.5)",
                "rgba(54, 162, 235, 0.5)",
                "rgba(255, 99, 132, 0.5)",
                "rgba(255, 206, 86, 0.5)",
                "rgba(153, 102, 255, 0.5)",
              ],
              borderColor: [
                "rgba(75, 192, 192, 1)",
                "rgba(54, 162, 235, 1)",
                "rgba(255, 99, 132, 1)",
                "rgba(255, 206, 86, 1)",
                "rgba(153, 102, 255, 1)",
              ],
              borderWidth: 1,
            },
          ],
        },
        options: {
          indexAxis: "y",
          responsive: true,
          maintainAspectRatio: true,
          plugins: {
            legend: {
              display: false,
            },
            title: {
              display: true,
              text: `Notification Statistics for ${email}`,
              padding: {
                top: 10,
                bottom: 30,
              },
            },
          },
          scales: {
            x: {
              beginAtZero: true,
              ticks: {
                stepSize: 1,
              },
            },
          },
        },
      });
    }
  } catch (error) {
    console.error(
      "Error loading user notification overview:",
      error.response?.data?.message || error.message
    );
  }
}

async function loadUserNotiTypeStats(email, type) {
  try {
    const response = await axios.get(
      `/api/NotiStatistics/user-stats-by-email-and-type`,
      {
        params: {
          email: email,
          type: type,
        },
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    if (response.data.success) {
      const stats = response.data.data[0] || {
        totalNotifications: 0,
        readCount: 0,
        unreadCount: 0,
        seenCount: 0,
        unseenCount: 0,
      };

      if (userNotiTypeStatsChart) {
        userNotiTypeStatsChart.destroy();
      }

      const ctx = $("#userNotiTypeStatsChart")[0].getContext("2d");
      userNotiTypeStatsChart = new Chart(ctx, {
        type: "polarArea",
        data: {
          labels: ["Read", "Unread", "Seen", "Unseen"],
          datasets: [
            {
              data: [
                stats.readCount || 0,
                stats.unreadCount || 0,
                stats.seenCount || 0,
                stats.unseenCount || 0,
              ],
              backgroundColor: [
                "rgba(54, 162, 235, 0.7)",
                "rgba(255, 99, 132, 0.7)",
                "rgba(255, 206, 86, 0.7)",
                "rgba(153, 102, 255, 0.7)",
              ],
              borderColor: [
                "rgba(54, 162, 235, 1)",
                "rgba(255, 99, 132, 1)",
                "rgba(255, 206, 86, 1)",
                "rgba(153, 102, 255, 1)",
              ],
              borderWidth: 1,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: true,
          scales: {
            r: {
              beginAtZero: true,
              ticks: {
                stepSize: 1,
              },
            },
          },
          plugins: {
            legend: {
              position: "right",
            },
            title: {
              display: true,
              text: [
                `${type} Notification Statistics for ${email}`,
                `Total Notifications: ${stats.totalNotifications}`,
              ],
              padding: {
                top: 10,
                bottom: 30,
              },
            },
          },
        },
      });
    }
  } catch (error) {
    console.error(
      "Error loading user notification type statistics:",
      error.response?.data?.message || error.message
    );
  }
}

async function getFilterOptions() {
  try {
    const response = await axios.get(
      "http://localhost:5001/api/PersonalProfile/filter-options",
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    if (response.data.success) {
      return response.data.data.map((item) => ({
        id: item.email,
        text: item.fullName ? `${item.fullName} (${item.email})` : item.email,
      }));
    }
    return [];
  } catch (error) {
    console.error(
      "Error loading email options:",
      error.response?.data?.message || error.message
    );
    alert("Unable to load email options. Please try refreshing the page.");
    return [];
  }
}

function updateUserNotiTypeStats() {
  const selectedEmail = $("#notiTypeSelect").val();
  const selectedType = $("#typeSelect").val();
  if (selectedEmail && selectedType) {
    loadUserNotiTypeStats(selectedEmail, selectedType);
  }
}

async function initializeSelects() {
  const data = await getFilterOptions();

  $("#notiSelect")
    .select2({
      width: "180px",
      placeholder: "Search for an email...",
      allowClear: true,
      dropdownParent: $("#notiSelectContainer"),
      data: data,
    })
    .val(null)
    .trigger("change");

  $("#userStatsSelect")
    .select2({
      width: "180px",
      placeholder: "Search for an email...",
      allowClear: true,
      dropdownParent: $("#userStatsSelectContainer"),
      data: data,
    })
    .val(null)
    .trigger("change");

  $("#notiTypeSelect")
    .select2({
      width: "180px",
      placeholder: "Search for an email...",
      allowClear: true,
      dropdownParent: $("#notiTypeSelectContainer"),
      data: data,
    })
    .val(null)
    .trigger("change");

  $("#typeSelect")
    .select2({
      width: "150px",
      placeholder: "Select type...",
      allowClear: true,
      dropdownParent: $("#typeSelectContainer"),
      data: [
        { id: "CongVan", text: "CongVan" },
        { id: "Lich", text: "Lich" },
        { id: "QuyTrinh", text: "QuyTrinh" },
      ],
    })
    .val(null)
    .trigger("change");
}

$(document).ready(async function () {
  const currentYear = new Date().getFullYear();

  await initializeSelects();

  const firstEmailOption = $("#notiSelect option:first").val();

  $("#notiSelect").val(firstEmailOption).trigger("change");
  $("#userStatsSelect").val(firstEmailOption).trigger("change");
  $("#notiTypeSelect").val(firstEmailOption).trigger("change");
  $("#typeSelect").val("CongVan").trigger("change");
  $("#yearInputMonthlyNoti").val(currentYear);
  $("#yearInputUserNoti").val(currentYear);

  loadNotiFrequencies();
  loadMonthlyNotiStats(currentYear);
  loadNotiTypeDistribution();
  loadUserNotiStats(firstEmailOption, currentYear);
  loadUserNotiOverview(firstEmailOption);
  loadUserNotiTypeStats(firstEmailOption, "CongVan");

  $("#yearInputMonthlyNoti").on("change", function () {
    const year = $(this).val();
    loadMonthlyNotiStats(year);
  });

  $("#yearInputUserNoti").on("change", function () {
    const year = $(this).val();
    const selectedEmail = $("#notiSelect").val();
    if (selectedEmail) {
      loadUserNotiStats(selectedEmail, year);
    }
  });

  $("#notiSelect").on("change", function () {
    const selectedEmail = $(this).val();
    const year = $("#yearInputUserNoti").val();
    if (selectedEmail) {
      loadUserNotiStats(selectedEmail, year);
    }
  });

  $("#userStatsSelect").on("change", function () {
    const selectedEmail = $(this).val();
    if (selectedEmail) {
      loadUserNotiOverview(selectedEmail);
    }
  });

  $("#typeSelect, #notiTypeSelect").on("change", updateUserNotiTypeStats);
});
