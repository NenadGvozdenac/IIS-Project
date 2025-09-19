import jsPDF from 'jspdf';
import 'jspdf-autotable';

export class PDFReportGenerator {
    constructor() {
        this.doc = new jsPDF();
        this.pageHeight = this.doc.internal.pageSize.height;
        this.pageWidth = this.doc.internal.pageSize.width;
        this.margin = 10; // Reduced from 20
        this.currentY = this.margin;
    }

    // Helper method to check if we need a new page
    checkPageBreak(requiredHeight) {
        if (this.currentY + requiredHeight > this.pageHeight - this.margin) {
            this.addPage();
            return true;
        }
        return false;
    }

    // Add new page and reset Y position
    addPage() {
        this.doc.addPage();
        this.currentY = this.margin;
        this.addHeader();
    }

    // Minimal header - black and white only
    addHeader() {
        // Simple black line
        this.doc.setDrawColor(0, 0, 0);
        this.doc.setLineWidth(0.5);
        this.doc.line(this.margin, this.margin + 15, this.pageWidth - this.margin, this.margin + 15);

        // Title - black text only
        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.setFontSize(14); // Reduced from 18
        this.doc.text('CLUB ANALYTICS REPORT', this.pageWidth / 2, this.margin + 10, { align: 'center' });

        this.currentY = this.margin + 25; // Reduced spacing
    }

    // Minimal title section
    addTitle(title, subtitle = '') {
        this.checkPageBreak(20); // Reduced from 30

        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.setFontSize(12); // Reduced from 16
        this.doc.text(title, this.margin, this.currentY);
        this.currentY += 8; // Reduced from 10

        if (subtitle) {
            this.doc.setFont('helvetica', 'normal');
            this.doc.setFontSize(8); // Reduced from 10
            this.doc.setTextColor(80, 80, 80); // Dark gray instead of color
            this.doc.text(subtitle, this.margin, this.currentY);
            this.currentY += 6; // Reduced from 8
        }

        this.currentY += 5; // Reduced from 10
    }

    // Minimal summary box - no background colors
    addSummaryBox(title, stats) {
        const boxHeight = Math.max(25, stats.length * 6 + 15); // Reduced height
        this.checkPageBreak(boxHeight + 5);

        // Simple border only - no background
        this.doc.setDrawColor(150, 150, 150); // Light gray border
        this.doc.setLineWidth(0.3);
        this.doc.rect(this.margin, this.currentY, this.pageWidth - 2 * this.margin, boxHeight);

        // Title
        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.setFontSize(10); // Reduced from 12
        this.doc.text(title, this.margin + 5, this.currentY + 8); // Reduced padding

        // Stats in compact layout
        let statsY = this.currentY + 15;
        stats.forEach((stat, index) => {
            if (index % 3 === 0 && index + 2 < stats.length) {
                // Three columns layout for more compact display
                const colWidth = (this.pageWidth - 2 * this.margin - 10) / 3;

                for (let i = 0; i < 3 && index + i < stats.length; i++) {
                    const currentStat = stats[index + i];
                    const xPos = this.margin + 5 + (i * colWidth);

                    this.doc.setFont('helvetica', 'normal');
                    this.doc.setFontSize(7); // Reduced font size
                    this.doc.setTextColor(100, 100, 100);
                    this.doc.text(currentStat.label + ':', xPos, statsY);

                    this.doc.setFont('helvetica', 'bold');
                    this.doc.setTextColor(0, 0, 0);
                    this.doc.text(currentStat.value, xPos, statsY + 4);
                }

                statsY += 10; // Reduced spacing
            }
        });

        // Handle remaining items if not divisible by 3
        const remaining = stats.length % 3;
        if (remaining > 0) {
            const startIndex = stats.length - remaining;
            const colWidth = (this.pageWidth - 2 * this.margin - 10) / remaining;

            for (let i = 0; i < remaining; i++) {
                const currentStat = stats[startIndex + i];
                const xPos = this.margin + 5 + (i * colWidth);

                this.doc.setFont('helvetica', 'normal');
                this.doc.setFontSize(7);
                this.doc.setTextColor(100, 100, 100);
                this.doc.text(currentStat.label + ':', xPos, statsY);

                this.doc.setFont('helvetica', 'bold');
                this.doc.setTextColor(0, 0, 0);
                this.doc.text(currentStat.value, xPos, statsY + 4);
            }
        }

        this.currentY += boxHeight + 8; // Reduced spacing
    }

    // Minimal table with autoTable
    addTable(headers, data, title = '') {
        if (title) {
            this.checkPageBreak(15);
            this.doc.setTextColor(0, 0, 0);
            this.doc.setFont('helvetica', 'bold');
            this.doc.setFontSize(10); // Reduced from 12
            this.doc.text(title, this.margin, this.currentY);
            this.currentY += 10; // Reduced from 15
        }

        this.doc.autoTable({
            head: [headers],
            body: data,
            startY: this.currentY,
            margin: { left: this.margin, right: this.margin },
            styles: {
                fontSize: 7, // Reduced from 8
                cellPadding: 2, // Reduced from 4
                overflow: 'linebreak',
                halign: 'left',
                textColor: [0, 0, 0] // Black text only
            },
            headStyles: {
                fillColor: [240, 240, 240], // Light gray instead of blue
                textColor: [0, 0, 0], // Black text
                fontStyle: 'bold',
                fontSize: 8 // Reduced from 9
            },
            alternateRowStyles: {
                fillColor: [250, 250, 250] // Very light gray
            },
            didDrawPage: (data) => {
                this.currentY = data.cursor.y + 5; // Reduced spacing
            }
        });
    }

    // Generate complete report PDF
    generateReport(reportsData) {
        // Initialize document
        this.addHeader();

        // Report metadata - minimal
        const reportDate = new Date().toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });

        this.doc.setTextColor(100, 100, 100); // Dark gray
        this.doc.setFont('helvetica', 'normal');
        this.doc.setFontSize(8); // Reduced from 10
        this.doc.text(`Generated: ${reportDate}`, this.margin, this.currentY);
        this.currentY += 15; // Reduced from 20

        // 1. EXECUTIVE SUMMARY - compact
        this.addTitle('EXECUTIVE SUMMARY', 'Key performance indicators');

        const executiveSummary = this.generateExecutiveSummary(reportsData);
        this.addSummaryBox('Overview', executiveSummary);

        // Page break before detailed sections
        this.doc.addPage();
        this.currentY = this.margin;

        // 2. MATCH TICKET SALES REPORT - minimal spacing
        this.addTitle('MATCH SALES ANALYSIS', 'Ticket sales by match');

        if (reportsData.matchSales.isSuccess) {
            const matchData = reportsData.matchSales.value;
            this.addSummaryBox('Sales Summary', [
                { label: 'Matches', value: matchData.totalMatches.toString() },
                { label: 'Tickets Sold', value: matchData.totalTicketsSold.toString() },
                { label: 'Total Revenue', value: `${matchData.totalRevenue.toLocaleString()} RSD` },
                { label: 'Avg/Match', value: `${(matchData.totalRevenue / Math.max(matchData.totalMatches, 1)).toLocaleString()} RSD` }
            ]);

            // Compact table
            const matchHeaders = ['Match', 'Date', 'Hall', 'Tickets', 'Revenue'];
            const matchTableData = matchData.matchSales.map(match => [
                match.matchName,
                new Date(match.scheduledAt).toLocaleDateString(),
                match.hall,
                match.totalTicketsSold.toString(),
                `${match.totalRevenue.toLocaleString()} RSD`
            ]);

            this.addTable(matchHeaders, matchTableData, 'Match Details');
        }

        // Page break before Customer Analysis
        this.doc.addPage();
        this.currentY = this.margin;

        // 3. CUSTOMER SPENDING REPORT - minimal
        this.addTitle('CUSTOMER ANALYSIS', 'Customer spending patterns');

        if (reportsData.customerSpending.isSuccess) {
            const customerData = reportsData.customerSpending.value;
            this.addSummaryBox('Customer Summary', [
                { label: 'Total Customers', value: customerData.totalCustomers.toString() },
                { label: 'Active Customers', value: customerData.totalActiveCustomers.toString() },
                { label: 'Total Spending', value: `${customerData.totalSpentByAllCustomers.toLocaleString()} RSD` },
                { label: 'Avg/Customer', value: `${customerData.averageSpendingPerCustomer.toLocaleString()} RSD` }
            ]);

            // Compact customer table
            const customerHeaders = ['Customer', 'Email', 'Tickets', 'Spent', 'Matches'];
            const customerTableData = customerData.customerSpending
                .filter(customer => customer.totalTicketsPurchased > 0)
                .map(customer => [
                    customer.customerName,
                    customer.email,
                    customer.totalTicketsPurchased.toString(),
                    `${customer.totalAmountSpent.toLocaleString()} RSD`,
                    customer.matchesAttended.toString()
                ]);

            this.addTable(customerHeaders, customerTableData, 'Active Customers');
        }

        // Page break before Sector Analysis
        this.doc.addPage();
        this.currentY = this.margin;

        // 4. SECTOR ANALYSIS REPORT - compact complex report
        this.addTitle('SECTOR ANALYSIS', 'Venue sector performance breakdown');

        if (reportsData.sectorAnalysis.isSuccess) {
            const sectorData = reportsData.sectorAnalysis.value.sectorAnalysis;

            // Minimal summary
            this.addSummaryBox('Sector Summary', [
                { label: 'Top Sector', value: sectorData.summary.mostPopularSector },
                { label: 'Revenue Leader', value: sectorData.summary.highestRevenueSector },
                { label: 'Premium Sector', value: sectorData.summary.highestAveragePriceSector },
                { label: 'Total Tickets', value: sectorData.summary.totalTicketsSoldAllSectors.toString() },
                { label: 'Total Revenue', value: `${sectorData.summary.totalRevenueAllSectors.toLocaleString()} RSD` },
                { label: 'Generated', value: sectorData.reportGeneratedAt }
            ]);

            // Compact sector table
            const sectorHeaders = ['Sector', 'Rank', 'Tickets', 'Avg Price', 'Revenue', 'Share %'];
            const sectorTableData = sectorData.sectorDetails.map(sector => [
                sector.sectorName.toUpperCase(),
                sector.popularityRank,
                sector.totalTicketsSold.toString(),
                `${sector.averagePrice.toLocaleString()} RSD`,
                `${sector.totalRevenue.toLocaleString()} RSD`,
                `${((sector.totalTicketsSold / sectorData.summary.totalTicketsSoldAllSectors) * 100).toFixed(1)}%`
            ]);

            this.addTable(sectorHeaders, sectorTableData, 'Sector Overview');
            
            // Page break before Customer Analysis
            this.doc.addPage();
            this.currentY = this.margin;

            // Minimal sector details
            sectorData.sectorDetails.forEach(sector => {
                this.addTitle(`${sector.sectorName.toUpperCase()} SECTOR`, `Match breakdown`);

                const matchHeaders = ['Match', 'Tickets', 'Price', 'Revenue', 'Share %'];
                const matchTableData = sector.matchAnalysis.map(match => [
                    match.matchName,
                    match.ticketsSold.toString(),
                    `${match.averagePrice.toLocaleString()} RSD`,
                    `${match.totalRevenue.toLocaleString()} RSD`,
                    `${match.salesPercentage.toFixed(1)}%`
                ]);

                this.addTable(matchHeaders, matchTableData);
            });
        }

        return this.doc;
    }

    generateExecutiveSummary(reportsData) {
        const summary = [];

        if (reportsData.matchSales.isSuccess) {
            const matchData = reportsData.matchSales.value;
            summary.push(
                { label: 'Total Revenue', value: `${matchData.totalRevenue.toLocaleString()} RSD` },
                { label: 'Matches Analyzed', value: matchData.totalMatches.toString() }
            );
        }

        if (reportsData.customerSpending.isSuccess) {
            const customerData = reportsData.customerSpending.value;
            summary.push(
                { label: 'Active Customers', value: customerData.totalActiveCustomers.toString() },
                { label: 'Avg. Customer Value', value: `${customerData.averageSpendingPerCustomer.toLocaleString()} RSD` }
            );
        }

        if (reportsData.sectorAnalysis.isSuccess) {
            const sectorData = reportsData.sectorAnalysis.value.sectorAnalysis;
            summary.push(
                { label: 'Top Performing Sector', value: sectorData.summary.mostPopularSector },
                { label: 'Premium Sector', value: sectorData.summary.highestAveragePriceSector }
            );
        }

        return summary;
    }

    // Save and download the PDF
    save(filename = 'club-analytics-report.pdf') {
        this.doc.save(filename);
    }
}