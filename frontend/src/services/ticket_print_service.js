// Ticket printing utility for frontend-only ticket generation
export class TicketPrintService {
  static async loadTemplate() {
    try {
      const response = await fetch('/src/assets/templates/ticket-template.html');
      return await response.text();
    } catch (error) {
      console.error('Failed to load ticket template:', error);
      // Fallback to inline template if loading fails
      return this.getFallbackTemplate();
    }
  }

  static getFallbackTemplate() {
    // Fallback template in case the file can't be loaded
    return `
      <!DOCTYPE html>
      <html>
      <head><title>Ticket - {{TICKET_NAME}}</title></head>
      <body style="font-family: Arial; padding: 20px;">
        <div style="width: 400px; height: 250px; border: 2px solid #2563eb; padding: 20px;">
          <h2>{{MATCH_NAME}}</h2>
          <p>Ticket Holder: {{USER_NAME}}</p>
          <p>{{ZONE_NAME}} - Row {{SEAT_ROW}}, Seat {{SEAT_NUMBER}} ({{SEAT_DIRECTION}} Side)</p>
          <p>Price: {{TICKET_PRICE}} RSD</p>
          <p>Valid until: {{VALID_UNTIL}}</p>
        </div>
      </body>
      </html>
    `;
  }

  static formatTemplate(template, data) {
    let formattedTemplate = template;
    
    // Replace all placeholders
    Object.keys(data).forEach(key => {
      const placeholder = `{{${key}}}`;
      const value = data[key] || '';
      formattedTemplate = formattedTemplate.replace(new RegExp(placeholder, 'g'), value);
    });
    
    return formattedTemplate;
  }

  static async printTicket(ticketData, userData) {
    try {
      // Load the template
      const template = await this.loadTemplate();
      
      // Prepare template data
      const templateData = {
        TICKET_NAME: ticketData.name,
        MATCH_NAME: ticketData.matchName || ticketData.name,
        USER_NAME: userData.userName,
        ZONE_NAME: ticketData.zoneName || 'General',
        SEAT_ROW: ticketData.seatRow,
        SEAT_NUMBER: ticketData.seatNumber,
        SEAT_DIRECTION: this.capitalizeFirst(ticketData.seatDirection || 'North'),
        TICKET_PRICE: this.formatPrice(ticketData.price),
        TICKET_ID: ticketData.idPurchaseOffer,
        PURCHASE_DATE: this.formatDate(ticketData.purchaseDate),
        VALID_UNTIL: this.formatDate(ticketData.expiresAt),
        TICKET_TYPE: ticketData.type,
        TICKET_TYPE_CLASS: this.getTicketTypeClass(ticketData.type),
        SEASON_INFO: ticketData.seasonName ? `
          <div class="detail-row">
            <span class="label">Season:</span>
            <span class="value">${ticketData.seasonName}</span>
          </div>
        ` : ''
      };
      
      // Format the template
      const ticketHTML = this.formatTemplate(template, templateData);
      
      // Calculate window dimensions (half of screen size)
      const screenWidth = window.screen.width;
      const screenHeight = window.screen.height;
      const windowWidth = Math.floor(screenWidth / 2);
      const windowHeight = Math.floor(screenHeight / 2);
      
      // Calculate center position
      const left = Math.floor((screenWidth - windowWidth) / 2);
      const top = Math.floor((screenHeight - windowHeight) / 2);
      
      // Create print window with calculated dimensions and position
      const printWindow = window.open('', '_blank', 
        `width=${windowWidth},height=${windowHeight},left=${left},top=${top},resizable=yes,scrollbars=yes`);
      
      if (!printWindow) {
        alert('Please allow popups for this site to print tickets.');
        return;
      }
      
      // Write HTML and print
      printWindow.document.write(ticketHTML);
      printWindow.document.close();
      
      printWindow.onload = function() {
        printWindow.print();
        printWindow.close();
      };
      
    } catch (error) {
      console.error('Error printing ticket:', error);
      alert('Failed to print ticket. Please try again.');
    }
  }

  static formatPrice(price) {
    if (!price) return '0.00';
    return parseFloat(price).toFixed(2);
  }

  static formatDate(dateString) {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  static getTicketTypeClass(type) {
    switch (type) {
      case 'season ticket':
        return 'season-ticket';
      case 'individual ticket':
        return 'individual-ticket';
      default:
        return 'default-ticket';
    }
  }

  static capitalizeFirst(str) {
    if (!str) return 'North';
    return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
  }
}
