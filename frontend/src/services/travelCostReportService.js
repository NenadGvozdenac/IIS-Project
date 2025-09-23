import jsPDF from 'jspdf'
import 'jspdf-autotable'
import axios from 'axios'
import { getUserData } from './auth_service.js'

export class TravelCostReportService {
  constructor() {
    this.doc = null
    this.pageWidth = 210 // A4 width in mm
    this.pageHeight = 297 // A4 height in mm
    this.margin = 20
  }

  /**
   * Generate travel cost report by fetching data from multiple endpoints
   */
  async generateTravelCostReport() {
    try {
      // Fetch data from backend APIs
      const reportData = await this.getTravelCostData()
      
      // Generate PDF with the fetched data
      this.doc = new jsPDF()
      
      console.log('Travel Cost Report Data:', reportData)
      
      // Add header
      this.addReportHeader()
      
      // Add individual match costs
      this.addIndividualMatchCosts(reportData.individualCosts)
      
      // Add summary of total costs
      this.addTotalCostsSummary(reportData.totalCosts)
      
      // Add footer
      this.addReportFooter()
      
      // Download the PDF
      const fileName = `Travel_Cost_Report_${this.formatDateForFilename(new Date())}.pdf`
      this.doc.save(fileName)
      
    } catch (error) {
      console.error('Error generating travel cost report:', error)
      throw error
    }
  }

  /**
   * Fetch travel cost data from backend API using new endpoint
   */
  async getTravelCostData() {
    try {
      // Call the new backend endpoint that uses PL/SQL function
      const response = await axios.get('https://localhost:5007/api/Trip/cost-report')
      
      console.log('Travel Cost Report Response:', response)
      
      if (response.status !== 200) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const result = response.data

      if (!result.isSuccess) {
        throw new Error(result.error || 'Failed to fetch travel cost report')
      }

      // The backend now returns the data in the expected format
      return result.value
      
    } catch (error) {
      console.error('Error fetching travel cost report:', error)
      throw error
    }
  }

  addReportHeader() {
    const centerX = this.pageWidth / 2

    // Main title
    this.doc.setFontSize(22)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('KK PARTIZAN', centerX, 25, { align: 'center' })
    
    this.doc.setFontSize(12)
    this.doc.setFont('helvetica', 'normal')
    this.doc.text('Professional Basketball Club', centerX, 32, { align: 'center' })
    
    this.doc.setFontSize(16)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('TRAVEL COST REPORT', centerX, 45, { align: 'center' })
    
    // Draw header separator line with gradient effect
    this.doc.setLineWidth(1)
    this.doc.setDrawColor(41, 128, 185)
    this.doc.line(this.margin, 50, this.pageWidth - this.margin, 50)
    
    // Report details with better styling
    const detailsYStart = 58
    this.doc.setFontSize(10)
    this.doc.setFont('helvetica', 'normal')
    this.doc.setTextColor(80, 80, 80)
    
    const reportDate = new Date().toLocaleDateString('en-US', { 
      weekday: 'long', 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    })

    this.doc.text(`Report Date: ${reportDate}`, centerX, detailsYStart, { align: 'center' })
    this.doc.text('Comprehensive analysis of travel expenses for away matches', centerX, detailsYStart + 7, { align: 'center' })
    
    // Reset text color
    this.doc.setTextColor(0, 0, 0)
  }

  addIndividualMatchCosts(individualCosts) {
    let currentY = 80

    this.doc.setFontSize(16)
    this.doc.setFont('helvetica', 'bold')
    this.doc.setTextColor(41, 128, 185)
    this.doc.text('MATCH-BY-MATCH BREAKDOWN', this.pageWidth / 2, currentY, { align: 'center' })
    this.doc.setTextColor(0, 0, 0)
    currentY += 12

    if (individualCosts.length === 0) {
      this.doc.setFontSize(12)
      this.doc.setFont('helvetica', 'normal')
      this.doc.text('No travel cost data available.', this.pageWidth / 2, currentY, { align: 'center' })
      return
    }

    for (let i = 0; i < individualCosts.length; i++) {
      const matchCost = individualCosts[i]
      
      // Check if we need a new page (more conservative spacing)
      if (currentY > 180) {
        this.doc.addPage()
        currentY = 30
      }
      
      // Match header with better styling
      this.doc.setFillColor(245, 245, 245)
      this.doc.rect(this.margin, currentY - 3, this.pageWidth - 2*this.margin, 20, 'F')

      this.doc.setFontSize(12)
      this.doc.setFont('helvetica', 'bold')
      this.doc.setTextColor(41, 128, 185)
      const matchDate = new Date(matchCost.match.date).toLocaleDateString('en-US', {
        weekday: 'short',
        year: 'numeric', 
        month: 'short', 
        day: 'numeric'
      })

      this.doc.text(`${matchCost.match.name}`, this.margin + 3, currentY + 4)

      // Split match title into two lines: "KK Partizan vs" and opponent on next line
      const clubLabel = 'KK Partizan vs'
      const opponent = matchCost.match.opponentTeam || ''

      this.doc.text(`${clubLabel}`, this.margin + 3, currentY + 11)
      this.doc.text(`${opponent}`, this.margin + 35, currentY + 11)
      // Date stays right-aligned at the first line's height
      this.doc.text(`${matchDate}`, this.pageWidth - this.margin - 3, currentY + 4, { align: 'right' })
      currentY += 16
      
      this.doc.setFontSize(10)
      this.doc.setFont('helvetica', 'normal')
      this.doc.setTextColor(80, 80, 80)
      this.doc.text(`Location: ${matchCost.match.city} - ${matchCost.match.hall}`, this.margin + 3, currentY + 4)
      this.doc.setTextColor(0, 0, 0)
      currentY += 12
      
      // Cost breakdown table with improved styling
      const costData = []

      if (matchCost.transportation.cost > 0) {
        const transportDetails = matchCost.transportation.details
        const detailsText = transportDetails ? `${transportDetails.company} (${transportDetails.type})` : 'Details not available'
        costData.push(['Transportation', detailsText, `$${matchCost.transportation.cost.toLocaleString('en-US')}`])
      }

      if (matchCost.accommodation.cost > 0) {
        const accommodationDetails = matchCost.accommodation.details
        const detailsText = accommodationDetails ? `${accommodationDetails.name} (${accommodationDetails.type})` : 'Details not available'
        costData.push(['Accommodation', detailsText, `$${matchCost.accommodation.cost.toLocaleString('en-US')}`])
      }
      
      // Total row with special styling
      costData.push(['TOTAL COST', '', `$${matchCost.totalCost.toLocaleString('en-US')}`])
      
      this.doc.autoTable({
        startY: currentY,
  head: [['Expense Type', 'Service Details', 'Amount']],
        body: costData,
        theme: 'striped',
        headStyles: { 
          fillColor: [41, 128, 185], 
          textColor: 255, 
          fontStyle: 'bold', 
          fontSize: 10,
          halign: 'center'
        },
        bodyStyles: { 
          fontSize: 9,
          cellPadding: 3
        },
        columnStyles: {
          // Make first column slightly wider so "Accommodation" fits on one line
          0: { cellWidth: 40, fontStyle: 'bold' },
          1: { cellWidth: 80 },
          2: { cellWidth: 30, halign: 'right', fontStyle: 'bold' }
        },
        margin: { left: this.margin, right: this.margin },
        didParseCell: function (data) {
          // Highlight total row
          if (data.row.index === costData.length - 1) {
            data.cell.styles.fillColor = [220, 220, 220]
            data.cell.styles.fontStyle = 'bold'
          }
        }
      })
      
      currentY = this.doc.lastAutoTable.finalY + 10
      
      // Improved travelers section with proper formatting and spacing
      if (matchCost.travelers.teamMembers.length > 0 || matchCost.travelers.management.length > 0) {
        
        // Check if we need more space for travelers section
        const estimatedHeight = this.calculateTravelersHeight(matchCost.travelers)
        if (currentY + estimatedHeight > 270) {
          this.doc.addPage()
          currentY = 30
        }
        
        // Create travelers box with border
        const boxStartY = currentY
        const boxWidth = this.pageWidth - 2 * this.margin
        
        // Background for travelers section
        this.doc.setFillColor(252, 252, 252)
        this.doc.rect(this.margin, currentY, boxWidth, estimatedHeight + 5, 'F')
        
        // Border around travelers section
        this.doc.setLineWidth(0.3)
        this.doc.setDrawColor(200, 200, 200)
        this.doc.rect(this.margin, currentY, boxWidth, estimatedHeight + 5)
        
        currentY += 5
        
        // Section title
        this.doc.setFontSize(10)
        this.doc.setFont('helvetica', 'bold')
        this.doc.setTextColor(41, 128, 185)
        this.doc.text('TRAVEL PARTICIPANTS', this.margin + 5, currentY)
        this.doc.setTextColor(0, 0, 0)
        currentY += 8
        
        // Team members with improved formatting
        if (matchCost.travelers.teamMembers.length > 0) {
          this.doc.setFontSize(9)
          this.doc.setFont('helvetica', 'bold')
          this.doc.setTextColor(60, 60, 60)
          this.doc.text('Team Members:', this.margin + 8, currentY)
          currentY += 5

          // Format team members in a clean list (3 per line)
          const teamMembers = matchCost.travelers.teamMembers
          currentY = this.addParticipantsList(teamMembers, currentY, this.margin + 12, { itemsPerLine: 3 })
        }

        // Management with same formatting as team members
        if (matchCost.travelers.management.length > 0) {
          if (matchCost.travelers.teamMembers.length > 0) {
            currentY += 3 // Extra space between sections
          }

          this.doc.setFontSize(9)
          this.doc.setFont('helvetica', 'bold')
          this.doc.setTextColor(60, 60, 60)
          this.doc.text('Management & Staff:', this.margin + 8, currentY)
          currentY += 5

          // Format management in a clean list (2 per line)
          const management = matchCost.travelers.management
          currentY = this.addParticipantsList(management, currentY, this.margin + 12, { itemsPerLine: 2 })
        }
        
        currentY += 8 // Bottom margin for the box
      }
      
      currentY += 20 // Space between matches
    }
  }

  /**
   * Calculate estimated height needed for travelers section
   */
  calculateTravelersHeight(travelers) {
    let height = 15 // Base height for title and padding

    if (travelers.teamMembers && travelers.teamMembers.length > 0) {
      const itemsPerLine = 3
      height += 8 // Title height
      height += Math.ceil(travelers.teamMembers.length / itemsPerLine) * 5 // Names in rows
    }

    if (travelers.management && travelers.management.length > 0) {
      const itemsPerLine = 2
      height += 8 // Title height
      height += Math.ceil(travelers.management.length / itemsPerLine) * 5 // Names in rows
    }

    return Math.max(height, 25) // Minimum height
  }

  /**
   * Add participants list with proper formatting - multiple people per line
   * options: { itemsPerLine: number }
   */
  addParticipantsList(participants, startY, leftMargin, options = {}) {
    let currentY = startY
    const lineHeight = 4.5
    const maxWidth = this.pageWidth - leftMargin - this.margin - 5

    const itemsPerLine = options.itemsPerLine || (participants.length > 8 ? 3 : 2)

    this.doc.setFontSize(9)
    this.doc.setFont('helvetica', 'normal')
    this.doc.setTextColor(80, 80, 80)

    for (let i = 0; i < participants.length; i += itemsPerLine) {
      const lineParticipants = participants.slice(i, i + itemsPerLine)
      let lineText = ''

      for (let j = 0; j < lineParticipants.length; j++) {
        const participant = String(lineParticipants[j] || '').trim()
        if (j === 0) {
          lineText = `• ${participant}`
        } else {
          lineText += ` • ${participant}`
        }
      }

      // Check if text fits on one line
      const textWidth = this.doc.getTextWidth(lineText)
      if (textWidth > maxWidth) {
        // If too long, split and wrap
        const wrappedText = this.doc.splitTextToSize(lineText, maxWidth)
        for (let k = 0; k < wrappedText.length; k++) {
          this.doc.text(wrappedText[k], leftMargin, currentY)
          currentY += lineHeight
        }
      } else {
        this.doc.text(lineText, leftMargin, currentY)
        currentY += lineHeight
      }
    }

    this.doc.setTextColor(0, 0, 0) // Reset color
    return currentY + 2 // Add small spacing after list
  }

  addTotalCostsSummary(totalCosts) {
    // Always start the financial summary on a fresh page to avoid overlapping
    this.doc.addPage()
    let currentY = 30

    // Section header with styling
    this.doc.setFontSize(16)
    this.doc.setFont('helvetica', 'bold')
    this.doc.setTextColor(41, 128, 185)
    this.doc.text('FINANCIAL SUMMARY', this.pageWidth / 2, currentY, { align: 'center' })
    this.doc.setTextColor(0, 0, 0)
    currentY += 20

    // Create summary data with better formatting
    const avgCostPerMatch = totalCosts.matchCount > 0 ? totalCosts.total / totalCosts.matchCount : 0
    
    const summaryData = [
      ['Total Transportation Expenses', `$${totalCosts.transportation.toLocaleString('en-US')}`],
      ['Total Accommodation Expenses', `$${totalCosts.accommodation.toLocaleString('en-US')}`],
      ['Number of Away Matches', totalCosts.matchCount.toString()],
      ['Average Cost per Match', `$${Math.round(avgCostPerMatch).toLocaleString('en-US')}`],
      ['GRAND TOTAL', `$${totalCosts.total.toLocaleString('en-US')}`]
    ]

    // Calculate table dimensions for better centering
    const tableWidth = 140
    const tableStartX = (this.pageWidth - tableWidth) / 2
    
    this.doc.autoTable({
      startY: currentY,
      body: summaryData,
      theme: 'striped',
      bodyStyles: { 
        fontSize: 11,
        fontStyle: 'bold',
        cellPadding: 4
      },
      columnStyles: {
        0: { 
          cellWidth: 90, 
          fillColor: [245, 245, 245],
          textColor: [80, 80, 80]
        },
        1: { 
          cellWidth: 50, 
          halign: 'right', 
          fillColor: [255, 255, 255],
          textColor: [0, 0, 0]
        }
      },
      margin: { left: tableStartX, right: tableStartX },
      didParseCell: function (data) {
        // Special styling for grand total row
        if (data.row.index === summaryData.length - 1) {
          data.cell.styles.fillColor = [41, 128, 185]
          data.cell.styles.textColor = [255, 255, 255]
          data.cell.styles.fontSize = 12
          data.cell.styles.fontStyle = 'bold'
        }
      }
    })

    // Add cost breakdown chart representation
    currentY = this.doc.lastAutoTable.finalY + 15
    
    if (totalCosts.total > 0) {
      this.doc.setFontSize(12)
      this.doc.setFont('helvetica', 'bold')
      this.doc.setTextColor(80, 80, 80)
      this.doc.text('Expense Distribution:', this.pageWidth / 2, currentY, { align: 'center' })
      currentY += 10
      
      const transportationPercentage = (totalCosts.transportation / totalCosts.total * 100).toFixed(1)
      const accommodationPercentage = (totalCosts.accommodation / totalCosts.total * 100).toFixed(1)
      
      this.doc.setFontSize(10)
      this.doc.setFont('helvetica', 'normal')
      this.doc.setTextColor(0, 0, 0)
      
      // Transportation bar with improved styling
      const barWidth = 120
      const barHeight = 10
      const barStartX = (this.pageWidth - barWidth) / 2
      
      // Transportation portion
      this.doc.setFillColor(41, 128, 185)
      this.doc.rect(barStartX, currentY, barWidth * (totalCosts.transportation / totalCosts.total), barHeight, 'F')
      
      // Accommodation portion
      this.doc.setFillColor(180, 180, 180)
      this.doc.rect(barStartX + barWidth * (totalCosts.transportation / totalCosts.total), currentY, 
                    barWidth * (totalCosts.accommodation / totalCosts.total), barHeight, 'F')
      
      // Border around bar
      this.doc.setLineWidth(0.5)
      this.doc.setDrawColor(100, 100, 100)
      this.doc.rect(barStartX, currentY, barWidth, barHeight)
      
      currentY += 15
      
      // Legend with better spacing
      this.doc.setFontSize(9)
      const legendStartX = (this.pageWidth - 120) / 2
      
      // Transportation legend
      this.doc.setFillColor(41, 128, 185)
      this.doc.rect(legendStartX, currentY - 2, 8, 4, 'F')
      this.doc.text(`Transportation: ${transportationPercentage}%`, legendStartX + 12, currentY)
      
      // Accommodation legend
      this.doc.setFillColor(180, 180, 180)
      this.doc.rect(legendStartX + 70, currentY - 2, 8, 4, 'F')
      this.doc.text(`Accommodation: ${accommodationPercentage}%`, legendStartX + 82, currentY)
    }
  }

  addReportFooter() {
    const finalY = this.doc.lastAutoTable ? this.doc.lastAutoTable.finalY : 200
    let footerY = Math.max(finalY + 30, this.pageHeight - 60)

    // If we're too close to the bottom, add a new page
    if (footerY > this.pageHeight - 60) {
      this.doc.addPage()
      footerY = this.pageHeight - 60
    }

    // Draw footer separator line
    this.doc.setLineWidth(1)
    this.doc.setDrawColor(41, 128, 185)
    this.doc.line(this.margin, footerY - 15, this.pageWidth - this.margin, footerY - 15)

    // Report generation info
    this.doc.setFontSize(10)
    this.doc.setFont('helvetica', 'bold')
    this.doc.setTextColor(41, 128, 185)
    
    const reportDate = new Date().toLocaleString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
    
    this.doc.text(`Report Generated: ${reportDate}`, this.pageWidth / 2, footerY, { align: 'center' })
    
    // User information
    this.doc.setFontSize(9)
    this.doc.setFont('helvetica', 'normal')
    this.doc.setTextColor(80, 80, 80)
    
    const userData = getUserData()
    const userName = userData ? `${userData.userName} (${userData.userEmail})` : 'Travel Management System'
    this.doc.text(`Generated by: ${userName}`, this.pageWidth / 2, footerY + 8, { align: 'center' })
    
    // Technical information
    this.doc.setFontSize(8)
    this.doc.setFont('helvetica', 'italic')
    this.doc.setTextColor(120, 120, 120)
    this.doc.text('Data Source: Travel Service API | Database: PostgreSQL with PL/SQL procedures', this.pageWidth / 2, footerY + 18, { align: 'center' })
    this.doc.text('This report includes all away matches with associated travel arrangements', this.pageWidth / 2, footerY + 24, { align: 'center' })
    
    // Reset colors
    this.doc.setTextColor(0, 0, 0)
    this.doc.setDrawColor(0, 0, 0)
  }

  formatDateForFilename(date) {
    return date.toISOString().split('T')[0] // Returns YYYY-MM-DD format
  }
}

export default new TravelCostReportService()