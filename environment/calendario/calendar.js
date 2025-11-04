class Calendar {
  constructor(element) {
    this.element = element;
    this.currentDate = new Date();
    this.render();
  }

  render() {
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth();

    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const startDate = new Date(firstDay);
    startDate.setDate(startDate.getDate() - firstDay.getDay());

    const daysInMonth = lastDay.getDate();
    const rows = Math.ceil((daysInMonth + firstDay.getDay()) / 7);
    const monthNames = [
      "Gennaio", "Febbraio", "Marzo", "Aprile", "Maggio", "Giugno",
      "Luglio", "Agosto", "Settembre", "Ottobre", "Novembre", "Dicembre"
    ];
    const dayNames = ["Dom", "Lun", "Mar", "Mer", "Gio", "Ven", "Sab"];

    let html = `
      <div class="calendar-header">
        <button id="prev-month">&lt;</button>
        <h2>${monthNames[month]} ${year}</h2>
        <button id="next-month">&gt;</button>
      </div>
      <table>
        <thead>
          <tr>
    `;

    dayNames.forEach(day => {
      html += `<th>${day}</th>`;
    });

    html += `
          </tr>
        </thead>
        <tbody>
    `;

    const today = new Date();
    for (let i = 0; i < rows; i++) {
      html += '<tr>';
      for (let j = 0; j < 7; j++) {
        const date = new Date(startDate);
        date.setDate(startDate.getDate() + i * 7 + j);
        const isCurrentMonth = date.getMonth() === month;
        const isToday = date.toDateString() === today.toDateString();
        const day = date.getDate();

        html += `<td class="${isCurrentMonth ? 'current-month' : 'other-month'} ${isToday ? 'today' : ''}">${day}</td>`;
      }
      html += '</tr>';
    }

    html += `
        </tbody>
      </table>
    `;

    this.element.innerHTML = html;

    // Add event listeners for navigation
    document.getElementById('prev-month').addEventListener('click', () => {
      this.currentDate.setMonth(this.currentDate.getMonth() - 1);
      this.render();
    });

    document.getElementById('next-month').addEventListener('click', () => {
      this.currentDate.setMonth(this.currentDate.getMonth() + 1);
      this.render();
    });
  }
}

// Initialize the calendar when the page loads
document.addEventListener('DOMContentLoaded', () => {
  const calendarElement = document.getElementById('calendar');
  new Calendar(calendarElement);
});