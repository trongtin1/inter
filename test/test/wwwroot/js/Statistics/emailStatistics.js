let monthlyChart;
let emailFrequencyChart;
let monthlyUserChart;

async function loadEmailFrequencies() {
  try {
    const response = await axios.get(
      "/api/MailStatistics/most-email-frequencies",
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    if (response.data.success) {
      const emails = response.data.data.map((item) => item.email);
      const counts = response.data.data.map((item) => item.count);

      if (emailFrequencyChart) {
        emailFrequencyChart.destroy();
      }

      const ctx = $("#emailFrequencyChart")[0].getContext("2d");
      emailFrequencyChart = new Chart(ctx, {
        type: "bar",
        data: {
          labels: emails,
          datasets: [
            {
              label: "Number of Emails",
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
              text: "Most Frequent Email Addresses",
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
      "Error loading email frequencies:",
      error.response?.data?.message || error.message
    );
  }
}

async function loadMonthlyStats(year) {
  try {
    const response = await axios.get(
      `/api/MailStatistics/email-frequencies-by-year`,
      {
        params: { year: year },
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    if (response.data.success) {
      const monthlyStats = response.data.data.monthlyStats;
      const totalEmails = monthlyStats.reduce(
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
      const labels = monthlyStats.map((stat) => monthNames[stat.month - 1]);
      const counts = monthlyStats.map((stat) => stat.count);

      if (monthlyChart) {
        monthlyChart.destroy();
      }

      const ctx = $("#monthlyChart")[0].getContext("2d");
      monthlyChart = new Chart(ctx, {
        type: "bar",
        data: {
          labels: labels,
          datasets: [
            {
              label: "Number of Emails",
              data: counts,
              backgroundColor: "rgba(54, 162, 235, 0.5)",
              borderColor: "rgba(54, 162, 235, 1)",
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
                `Monthly Email Statistics - ${year}`,
                `Total Emails: ${totalEmails}`,
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

async function loadUserStats(email, year) {
  try {
    const response = await axios.get(
      `/api/MailStatistics/email-monthly-stats`,
      {
        params: {
          email: email,
          year: year,
        },
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    if (response.data.success) {
      const monthlyStats = response.data.data.monthlyStats;
      const totalEmails = monthlyStats.reduce(
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
      const labels = monthlyStats.map((stat) => monthNames[stat.month - 1]);
      const counts = monthlyStats.map((stat) => stat.count);

      if (monthlyUserChart) {
        monthlyUserChart.destroy();
      }

      const ctx = $("#monthlyUserChart")[0].getContext("2d");
      monthlyUserChart = new Chart(ctx, {
        type: "bar",
        data: {
          labels: labels,
          datasets: [
            {
              label: "Number of Emails",
              data: counts,
              backgroundColor: "rgba(153, 102, 255, 0.5)",
              borderColor: "rgba(153, 102, 255, 1)",
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
                `Monthly Email Statistics - ${email} (${year})`,
                `Total Emails: ${totalEmails}`,
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

async function initializeEmailSelect() {
  try {
    const response = await axios.get("/api/Mails/filter-options", {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    });

    if (response.data.success) {
      const data = response.data.data;

      const uniqueEmails = new Set([...data.emails]);

      const emailOptions = Array.from(uniqueEmails).map((email) => ({
        id: email,
        text: email,
      }));

      $("#emailSelect")
        .select2({
          data: emailOptions,
          width: "180px",
          placeholder: "Search for an email...",
          allowClear: true,
          dropdownParent: $("#emailSelectContainer"),
        })
        .val(null)
        .trigger("change");
    }
  } catch (error) {
    console.error(
      "Error loading filter options:",
      error.response?.data?.message || error.message
    );
    alert("Unable to load email options. Please try refreshing the page.");
  }
}

$(document).ready(async function () {
  const currentYear = new Date().getFullYear();

  await initializeEmailSelect();

  const firstEmailOption = $("#emailSelect option:first").val();

  $("#emailSelect").val(firstEmailOption).trigger("change");

  $("#yearInputMonthly").val(currentYear);
  $("#yearInputUser").val(currentYear);

  loadEmailFrequencies();
  loadMonthlyStats(currentYear);
  loadUserStats(firstEmailOption, currentYear);

  $("#yearInputMonthly").on("change", function () {
    const year = $(this).val();
    loadMonthlyStats(year);
  });

  $("#yearInputUser").on("change", function () {
    const year = $(this).val();
    const selectedEmail = $("#emailSelect").val();
    if (selectedEmail) {
      loadUserStats(selectedEmail, year);
    }
  });

  $("#emailSelect").on("change", function () {
    const selectedEmail = $(this).val();
    const year = $("#yearInputUser").val();
    if (selectedEmail) {
      loadUserStats(selectedEmail, year);
    }
  });
});
