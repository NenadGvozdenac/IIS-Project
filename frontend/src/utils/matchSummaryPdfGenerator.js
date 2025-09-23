import jsPDF from 'jspdf';
import 'jspdf-autotable';

export class MatchSummaryPdfGenerator {
    constructor() {
        this.doc = new jsPDF();
        this.pageHeight = this.doc.internal.pageSize.height;
        this.pageWidth = this.doc.internal.pageSize.width;
        this.margin = 15;
        this.currentY = this.margin;
    }

    addHeader() {
        // Header line
        this.doc.setDrawColor(0, 0, 0);
        this.doc.setLineWidth(0.8);
        this.doc.line(this.margin, this.margin + 15, this.pageWidth - this.margin, this.margin + 15);

        // Title
        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.setFontSize(16);
        this.doc.text('MATCH SUMMARY REPORT', this.pageWidth / 2, this.margin + 10, { align: 'center' });

        // Subtitle
        this.doc.setFont('helvetica', 'normal');
        this.doc.setFontSize(10);
        this.doc.setTextColor(80, 80, 80);
        const reportDate = new Date().toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
        this.doc.text(`Generated: ${reportDate}`, this.pageWidth / 2, this.margin + 20, { align: 'center' });

        this.currentY = this.margin + 35;
    }

    addSummaryStats(matches) {
        // Calculate summary statistics
        const totalMatches = matches.length;
        const totalTicketsSold = matches.reduce((sum, match) => sum + match.totalTicketsSold, 0);
        const totalRevenue = matches.reduce((sum, match) => sum + match.totalRevenue, 0);
        const avgFillPercentage = matches.reduce((sum, match) => sum + match.stadiumFillPercentage, 0) / totalMatches;
        const avgTicketPrice = totalTicketsSold > 0 ? totalRevenue / totalTicketsSold : 0;

        // Summary box
        const boxHeight = 35;
        this.doc.setDrawColor(150, 150, 150);
        this.doc.setLineWidth(0.5);
        this.doc.rect(this.margin, this.currentY, this.pageWidth - 2 * this.margin, boxHeight);

        // Summary title
        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.setFontSize(12);
        this.doc.text('EXECUTIVE SUMMARY', this.margin + 5, this.currentY + 8);

        // Summary stats in two columns
        const statsY = this.currentY + 18;
        const leftCol = this.margin + 10;
        const rightCol = this.pageWidth / 2 + 10;

        // Left column
        this.doc.setFont('helvetica', 'normal');
        this.doc.setFontSize(9);
        this.doc.setTextColor(100, 100, 100);

        this.doc.text('Total Matches:', leftCol, statsY);
        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.text(totalMatches.toString(), leftCol + 40, statsY);

        this.doc.setFont('helvetica', 'normal');
        this.doc.setTextColor(100, 100, 100);
        this.doc.text('Tickets Sold:', leftCol, statsY + 8);
        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.text(totalTicketsSold.toLocaleString(), leftCol + 40, statsY + 8);

        // Right column
        this.doc.setFont('helvetica', 'normal');
        this.doc.setTextColor(100, 100, 100);
        this.doc.text('Total Revenue:', rightCol, statsY);
        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.text(`${totalRevenue.toLocaleString()} RSD`, rightCol + 40, statsY);

        this.doc.setFont('helvetica', 'normal');
        this.doc.setTextColor(100, 100, 100);
        this.doc.text('Avg Fill Rate:', rightCol, statsY + 8);
        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.text(`${avgFillPercentage.toFixed(1)}%`, rightCol + 40, statsY + 8);

        this.currentY += boxHeight + 15;
    }

    addMatchDetailsTable(matches) {
        // Table title
        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.setFontSize(12);
        this.doc.text('MATCH DETAILS', this.margin, this.currentY);
        this.currentY += 10;

        // Prepare table data
        const headers = [
            'Match',
            'Date',
            'Type',
            'Hall',
            'Tickets',
            'Revenue',
            'Fill %',
            'VIP Fill %',
            'Regular Fill %'
        ];

        const tableData = matches.map(match => [
            match.matchName,
            new Date(match.matchDate).toLocaleDateString('en-US', { 
                month: 'short', 
                day: 'numeric' 
            }),
            match.matchType,
            match.hall,
            match.totalTicketsSold.toString(),
            `${(match.totalRevenue / 1000).toFixed(0)}k`,
            `${match.stadiumFillPercentage.toFixed(1)}%`,
            `${match.vipZoneFillPercentage.toFixed(1)}%`,
            `${match.regularZoneFillPercentage.toFixed(1)}%`
        ]);

        // Generate table
        this.doc.autoTable({
            head: [headers],
            body: tableData,
            startY: this.currentY,
            margin: { left: this.margin, right: this.margin },
            styles: {
                fontSize: 7,
                cellPadding: 2,
                overflow: 'linebreak',
                halign: 'center',
                textColor: [0, 0, 0]
            },
            headStyles: {
                fillColor: [230, 230, 230],
                textColor: [0, 0, 0],
                fontStyle: 'bold',
                fontSize: 8,
                halign: 'center'
            },
            alternateRowStyles: {
                fillColor: [248, 248, 248]
            },
            columnStyles: {
                0: { halign: 'left', cellWidth: 35 }, // Match name
                1: { halign: 'center', cellWidth: 20 }, // Date
                2: { halign: 'center', cellWidth: 15 }, // Type
                3: { halign: 'left', cellWidth: 25 }, // Hall
                4: { halign: 'right', cellWidth: 15 }, // Tickets
                5: { halign: 'right', cellWidth: 18 }, // Revenue
                6: { halign: 'right', cellWidth: 15 }, // Fill %
                7: { halign: 'right', cellWidth: 18 }, // VIP Fill %
                8: { halign: 'right', cellWidth: 20 }  // Regular Fill %
            },
            didDrawPage: (data) => {
                this.currentY = data.cursor.y + 10;
            }
        });
    }

    addZoneAnalysis(matches) {
        // Zone Analysis title
        this.doc.setTextColor(0, 0, 0);
        this.doc.setFont('helvetica', 'bold');
        this.doc.setFontSize(12);
        this.doc.text('ZONE PERFORMANCE ANALYSIS', this.margin, this.currentY);
        this.currentY += 15;

        // Calculate zone statistics
        const totalVipTickets = matches.reduce((sum, match) => sum + match.vipZoneTickets, 0);
        const totalRegularTickets = matches.reduce((sum, match) => sum + match.regularZoneTickets, 0);
        const totalVipRevenue = matches.reduce((sum, match) => sum + match.vipZoneRevenue, 0);
        const totalRegularRevenue = matches.reduce((sum, match) => sum + match.regularZoneRevenue, 0);

        const avgVipFill = matches.reduce((sum, match) => sum + match.vipZoneFillPercentage, 0) / matches.length;
        const avgRegularFill = matches.reduce((sum, match) => sum + match.regularZoneFillPercentage, 0) / matches.length;

        // Zone comparison table
        const zoneHeaders = ['Zone Type', 'Tickets Sold', 'Total Revenue', 'Avg Fill Rate', 'Revenue Share'];
        const zoneData = [
            [
                'VIP Zone',
                totalVipTickets.toString(),
                `${totalVipRevenue.toLocaleString()} RSD`,
                `${avgVipFill.toFixed(1)}%`,
                `${((totalVipRevenue / (totalVipRevenue + totalRegularRevenue)) * 100).toFixed(1)}%`
            ],
            [
                'Regular Zone',
                totalRegularTickets.toString(),
                `${totalRegularRevenue.toLocaleString()} RSD`,
                `${avgRegularFill.toFixed(1)}%`,
                `${((totalRegularRevenue / (totalVipRevenue + totalRegularRevenue)) * 100).toFixed(1)}%`
            ]
        ];

        this.doc.autoTable({
            head: [zoneHeaders],
            body: zoneData,
            startY: this.currentY,
            margin: { left: this.margin, right: this.margin },
            styles: {
                fontSize: 9,
                cellPadding: 3,
                textColor: [0, 0, 0],
                halign: 'center'
            },
            headStyles: {
                fillColor: [220, 220, 220],
                textColor: [0, 0, 0],
                fontStyle: 'bold',
                fontSize: 10
            },
            columnStyles: {
                0: { halign: 'left' },
                1: { halign: 'right' },
                2: { halign: 'right' },
                3: { halign: 'right' },
                4: { halign: 'right' }
            },
            didDrawPage: (data) => {
                this.currentY = data.cursor.y + 10;
            }
        });
    }

    generateReport(matchSummaryData) {
        // Initialize document
        this.addHeader();

        // Add content sections
        this.addSummaryStats(matchSummaryData.matches);
        this.addMatchDetailsTable(matchSummaryData.matches);
        this.addZoneAnalysis(matchSummaryData.matches);

        // Footer with generation info
        this.doc.setFontSize(8);
        this.doc.setTextColor(120, 120, 120);
        this.doc.text(
            `Report contains ${matchSummaryData.matches.length} matches • Generated using complex PL/SQL analysis`,
            this.pageWidth / 2,
            this.pageHeight - 10,
            { align: 'center' }
        );

        return this.doc;
    }

    save(filename = 'match-summary-report.pdf') {
        this.doc.save(filename);
    }
}