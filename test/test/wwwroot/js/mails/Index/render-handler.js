const renderHandler = {
  renderTable(items) {
    const tbody = $("#mailsTableBody");
    tbody.empty();

    items.forEach((mail) => {
      const row = `
                <tr>
                    <td>
                        <input type="checkbox" class="form-check-input mail-checkbox" value="${
                          mail.id
                        }" />
                    </td>
                    <td><a href="/Mails/Details/${mail.id}">${mail.id}</a></td>
                    <td><span class="badge bg-secondary">${
                      mail.email || ""
                    }</span></td>
                    <td>${formatHandler.formatEmailContent(
                      mail.emailContent
                    )}</td>
                    <td>${mail.fileAttach || ""}</td>
                    <td>${mail.createBy || ""}</td>
                    <td>${formatHandler.formatDateTime(mail.createTime)}</td>
                    <td>${mail.isSend ? "Yes" : "No"}</td>
                    <td>${formatHandler.formatDateTime(mail.sendTime)}</td>
                    <td>${mail.subject || ""}</td>
                    <td>${mail.sentStatus || ""}</td>
                    <td>${formatHandler.formatEmailList(mail.emailCc)}</td>
                    <td>${formatHandler.formatEmailList(mail.emailBcc)}</td>
                </tr>
            `;
      tbody.append(row);
    });

    // Reset select all checkbox state
    $("#selectAll").prop("checked", false);
  },
  renderPagination(data) {
    const pagination = $("#pagination");
    pagination.empty();
    totalPages = data.totalPages;
    currentPage = data.pageIndex;

    if (totalPages > 1) {
      // Previous button
      pagination.append(`
                <li class="page-item ${data.hasPreviousPage ? "" : "disabled"}">
                    <a class="page-link" href="#" onclick="changePage(${
                      currentPage - 1
                    })">Previous</a>
                </li>
            `);

      // Page numbers
      for (
        let i = Math.max(1, currentPage - 2);
        i <= Math.min(totalPages, currentPage + 2);
        i++
      ) {
        pagination.append(`
                    <li class="page-item ${i === currentPage ? "active" : ""}">
                        <a class="page-link" href="#" onclick="changePage(${i})">${i}</a>
                    </li>
                `);
      }

      // Next button
      pagination.append(`
                <li class="page-item ${data.hasNextPage ? "" : "disabled"}">
                    <a class="page-link" href="#" onclick="changePage(${
                      currentPage + 1
                    })">Next</a>
                </li>
            `);
    }
  },
};
