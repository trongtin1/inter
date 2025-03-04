async function exportFile() {
  try {
    const defaultFileName = `Mails_Export_${new Date()
      .toISOString()
      .slice(0, 10)}.xlsx`;
    const filters = filterHandler.getFilterValues();

    // Build query parameters with proper date formatting
    const params = new URLSearchParams();
    params.append("fileName", defaultFileName);

    // Add other filter parameters
    Object.entries(filters).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== "") {
        // Format dates properly if they exist
        if (value instanceof Date) {
          params.append(key, value.toISOString());
        } else {
          params.append(key, value);
        }
      }
    });

    // Add error handling for the API call
    let response;
    try {
      response = await axios.get(
        `${mailApiService.baseUrl}/mail-excel/export?${params}`,
        {
          headers: {
            Authorization: `Bearer ${mailApiService.getToken()}`,
          },
          responseType: "blob",
        }
      );
    } catch (error) {
      throw new Error(
        error.response?.data?.message || "Failed to connect to the server"
      );
    }

    // Validate response
    if (!response.data || response.data.size === 0) {
      throw new Error("Received empty response from server");
    }

    const blob = new Blob([response.data], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    });

    if (window.showSaveFilePicker) {
      try {
        const handle = await window.showSaveFilePicker({
          suggestedName: defaultFileName,
          types: [
            {
              description: "Excel Files",
              accept: {
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                  [".xlsx"],
              },
            },
          ],
        });
        const writable = await handle.createWritable();
        await writable.write(blob);
        await writable.close();
      } catch (err) {
        if (err.name === "AbortError") return;
        throw err;
      }
    } else {
      const url = window.URL.createObjectURL(blob);
      const $a = $("<a>", {
        href: url,
        download: defaultFileName,
      });
      $("body").append($a);
      $a[0].click(); // Sử dụng jQuery để click
      $a.remove(); // Xóa phần tử sau khi click
      window.URL.revokeObjectURL(url);
    }
  } catch (error) {
    console.error("Export error:", error);
    alert(
      "Error exporting file: " +
        (error.response?.data?.message || error.message)
    );
  }
}

async function importFile() {
  // Tạo input file ẩn bằng jQuery
  const $fileInput = $("<input>", {
    type: "file",
    accept: ".xlsx",
  });

  // Gán sự kiện onchange
  $fileInput.on("change", async function (e) {
    try {
      const file = e.target.files[0];
      if (!file) return;

      // Kiểm tra định dạng file
      if (!file.name.toLowerCase().endsWith(".xlsx")) {
        alert("Please select an Excel (.xlsx) file");
        return;
      }

      // Tạo FormData
      const formData = new FormData();
      formData.append("file", file);

      // Gửi file đến server
      const response = await axios.post(
        `${mailApiService.baseUrl}/mail-excel/import`,
        formData,
        {
          headers: {
            Authorization: `Bearer ${mailApiService.getToken()}`,
            "Content-Type": "multipart/form-data",
          },
        }
      );

      if (response.data.success) {
        alert(`Import completed:\n${response.data.message}`);
        // Xóa cache và làm mới dữ liệu
        // sessionStorage.removeItem("filterOptions");
        await filterHandler.loadFilterOptions();
        await loadData();
      } else {
        alert("Import failed: " + response.data.message);
      }
    } catch (error) {
      console.error("Import error:", error);
      alert(
        "Error importing file: " +
          (error.response?.data?.message || error.message)
      );
    }
  });

  // Mở hộp thoại chọn file
  $fileInput.click(); // Đặt ở cuối để đảm bảo mọi thứ đã sẵn sàng
}
