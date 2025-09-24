import jsPDF from 'jspdf'
import 'jspdf-autotable'
import axios from 'axios'
import { MATCHES_URL } from './const_service'

export class InfluxReportService {
  constructor() {
    this.doc = null
    this.pageWidth = 210 // A4 width in mm
    this.pageHeight = 297 // A4 height in mm
    this.margin = 20
  }

  /**
   * Generate InfluxDB Analytics Report by fetching data from backend APIs
   * @param {Object} params - Report parameters
   * @param {string} params.playerId - The ID of the player
   * @param {string} params.matchId - The ID of the match
   * @param {string} params.startDate - Start date for analysis
   * @param {string} params.endDate - End date for analysis
   * @param {string} params.teamId - Team ID (default: "1")
   */
  async generateInfluxReport({ playerId, matchId, startDate, endDate, teamId = "1" }) {
    try {
      console.log('Generating InfluxDB report with params:', { playerId, matchId, startDate, endDate, teamId })

      // Initialize PDF document
      this.doc = new jsPDF()

      // Fetch all three reports from backend
      const [scoringEvents, playerEventCounts, teamPlayerAverages] = await Promise.all([
        this.fetchMatchScoringEvents(matchId),
        this.fetchPlayerEventCounts(playerId, startDate, endDate),
        this.fetchTeamPlayerAverages(startDate, endDate, teamId)
      ])

      console.log('Fetched data:', {
        scoringEvents: scoringEvents.length,
        playerEventCounts: playerEventCounts.length,
        teamPlayerAverages: teamPlayerAverages.length
      })

      // Add report header
      this.addReportHeader({ playerId, matchId, startDate, endDate, teamId })

      // Add Match Scoring Events section
      this.addMatchScoringEventsSection(scoringEvents, matchId)

      // Add Player Event Counts section  
      this.addPlayerEventCountsSection(playerEventCounts, playerId)

      // Add Team Player Averages section
      this.addTeamPlayerAveragesSection(teamPlayerAverages, teamId, startDate, endDate)

      // Add footer
      this.addReportFooter()

      // Download the PDF
      const fileName = `InfluxDB_Analytics_Report_Player${playerId}_Match${matchId}_${this.formatDateForFilename(startDate)}_to_${this.formatDateForFilename(endDate)}.pdf`
      this.doc.save(fileName)

    } catch (error) {
      console.error('Error generating InfluxDB analytics report:', error)
      throw error
    }
  }

  /**
   * Fetch match scoring events from backend API
   */
  async fetchMatchScoringEvents(matchId) {
    try {
      const response = await axios.get(`${MATCHES_URL}/ChronologicalEventInflux/reports/match/${matchId}/scoring-events`)
      console.log('Match scoring events response:', response.data)
      return response.data || []
    } catch (error) {
      console.error('Error fetching match scoring events:', error)
      throw new Error(`Failed to fetch scoring events: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Fetch player event counts from backend API
   */
  async fetchPlayerEventCounts(playerId, startDate, endDate) {
    try {
      const response = await axios.get(`${MATCHES_URL}/ChronologicalEventInflux/reports/player/${playerId}/event-counts`, {
        params: {
          startDate: startDate,
          endDate: endDate
        }
      })
      return response.data || []
    } catch (error) {
      console.error('Error fetching player event counts:', error)
      throw new Error(`Failed to fetch player event counts: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Fetch team player averages from backend API
   */
  async fetchTeamPlayerAverages(startDate, endDate, teamId) {
    try {
      const response = await axios.get(`${MATCHES_URL}/ChronologicalEventInflux/reports/team/player-averages`, {
        params: {
          startDate: startDate,
          endDate: endDate,
          teamId: teamId
        }
      })
      return response.data || []
    } catch (error) {
      console.error('Error fetching team player averages:', error)
      throw new Error(`Failed to fetch team player averages: ${error.response?.data?.message || error.message}`)
    }
  }

  addReportHeader({ playerId, matchId, startDate, endDate, teamId }) {
    const centerX = this.pageWidth / 2

    // Main title
    this.doc.setFontSize(20)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('KK PARTIZAN', centerX, 25, { align: 'center' })
    
    this.doc.setFontSize(12)
    this.doc.setFont('helvetica', 'normal')
    this.doc.text('Professional Basketball Association', centerX, 32, { align: 'center' })
    
    this.doc.setFontSize(14)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('INFLUXDB ANALYTICS REPORT', centerX, 45, { align: 'center' })
    
    // Draw header separator line
    this.doc.setLineWidth(0.5)
    this.doc.line(this.margin, 50, this.pageWidth - this.margin, 50)
    
    // Report parameters
    const detailsYStart = 58
    this.doc.setFontSize(10)
    this.doc.setFont('helvetica', 'normal')
    
    const formattedStartDate = new Date(startDate).toLocaleDateString('en-US')
    const formattedEndDate = new Date(endDate).toLocaleDateString('en-US')

    const detailsCenterX = centerX - 40
    this.doc.text(`Analysis Period: ${formattedStartDate} - ${formattedEndDate}`, detailsCenterX, detailsYStart)
    this.doc.text(`Player ID: ${playerId} | Match ID: ${matchId} | Team ID: ${teamId}`, detailsCenterX, detailsYStart + 7)
    this.doc.text(`Generated: ${new Date().toLocaleString('en-US')}`, detailsCenterX, detailsYStart + 14)
  }

  addMatchScoringEventsSection(scoringEvents, matchId) {
    let currentY = 85

    // Section title
    this.doc.setFontSize(14)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('1. MATCH SCORING EVENTS', this.margin, currentY)
    
    currentY += 5
    this.doc.setFontSize(10)
    this.doc.setFont('helvetica', 'normal')
    this.doc.text(`Scoring events for Match ${matchId} (event_type: +2p, +3p, +ft | event_category: personal)`, this.margin, currentY)
    
    currentY += 10

    if (scoringEvents.length === 0) {
      this.doc.setFont('helvetica', 'italic')
      this.doc.text('No scoring events found for this match.', this.margin, currentY)
      currentY += 15
    } else {
      // Prepare table data
      const tableData = scoringEvents.map(event => [
        new Date(event.timestamp).toLocaleTimeString('en-US', { hour12: false }),
        event.playerName || event.playerId || 'Unknown',
        event.eventType || '',
        event.period || '',
        this.formatPeriodTime(event.periodTime || 0),
        (event.eventId || '').toString()
      ])

      this.doc.autoTable({
        startY: currentY,
        head: [['Time', 'Player', 'Event Type', 'Period', 'Period Time', 'Event ID']],
        body: tableData,
        theme: 'grid',
        headStyles: { 
          fillColor: [41, 128, 185], 
          textColor: 255, 
          fontStyle: 'bold', 
          fontSize: 8,
          halign: 'center' 
        },
        bodyStyles: { 
          fontSize: 7,
          halign: 'center'
        },
        columnStyles: {
          0: { cellWidth: 20, halign: 'center' },
          1: { cellWidth: 35, halign: 'left' },
          2: { cellWidth: 25, halign: 'center' },
          3: { cellWidth: 20, halign: 'center' },
          4: { cellWidth: 25, halign: 'center' },
          5: { cellWidth: 25, halign: 'center' }
        },
        margin: { left: this.margin, right: this.margin }
      })

      currentY = this.doc.lastAutoTable.finalY + 15
    }

    return currentY
  }

  addPlayerEventCountsSection(playerEventCounts, playerId) {
    let currentY = this.doc.lastAutoTable ? this.doc.lastAutoTable.finalY + 20 : 120

    // Check if we need a new page
    if (currentY > 200) {
      this.doc.addPage()
      currentY = 30
    }

    // Section title
    this.doc.setFontSize(14)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('2. PLAYER EVENT COUNTS BY MATCH', this.margin, currentY)
    
    currentY += 5
    this.doc.setFontSize(10)
    this.doc.setFont('helvetica', 'normal')
    
    const playerName = playerEventCounts.length > 0 ? playerEventCounts[0].playerName : 'Unknown'
    this.doc.text(`Event statistics for Player ${playerId} (${playerName})`, this.margin, currentY)
    
    currentY += 10

    if (playerEventCounts.length === 0) {
      this.doc.setFont('helvetica', 'italic')
      this.doc.text('No event data found for this player in the specified period.', this.margin, currentY)
      currentY += 15
    } else {
      // Prepare table data
      const tableData = playerEventCounts.map(stats => [
        stats.matchId || '',
        (stats.twoPointers || 0).toString(),
        (stats.threePointers || 0).toString(),
        (stats.freeThrows || 0).toString(),
        (stats.reboundsOffensive || 0).toString(),
        (stats.reboundsDefensive || 0).toString(),
        (stats.assists || 0).toString(),
        (stats.fouls || 0).toString(),
        (stats.steals || 0).toString(),
        (stats.blocks || 0).toString(),
        (stats.totalEvents || 0).toString()
      ])

      this.doc.autoTable({
        startY: currentY,
        head: [['Match ID', '2P', '3P', 'FT', 'REB-O', 'REB-D', 'AST', 'PF', 'STL', 'BLK', 'Total']],
        body: tableData,
        theme: 'grid',
        headStyles: { 
          fillColor: [52, 73, 94], 
          textColor: 255, 
          fontStyle: 'bold', 
          fontSize: 8,
          halign: 'center' 
        },
        bodyStyles: { 
          fontSize: 7,
          halign: 'center'
        },
        columnStyles: {
          0: { cellWidth: 25, halign: 'center' },
          1: { cellWidth: 15, halign: 'center' },
          2: { cellWidth: 15, halign: 'center' },
          3: { cellWidth: 15, halign: 'center' },
          4: { cellWidth: 15, halign: 'center' },
          5: { cellWidth: 15, halign: 'center' },
          6: { cellWidth: 15, halign: 'center' },
          7: { cellWidth: 15, halign: 'center' },
          8: { cellWidth: 15, halign: 'center' },
          9: { cellWidth: 15, halign: 'center' },
          10: { cellWidth: 15, halign: 'center' },
          11: { cellWidth: 20, halign: 'center', fontStyle: 'bold' }
        },
        margin: { left: this.margin, right: this.margin }
      })
    }

    return currentY
  }

  addTeamPlayerAveragesSection(teamPlayerAverages, teamId, startDate, endDate) {
    let currentY = this.doc.lastAutoTable ? this.doc.lastAutoTable.finalY + 20 : 160

    // Check if we need a new page
    if (currentY > 200) {
      this.doc.addPage()
      currentY = 30
    }

    // Section title
    this.doc.setFontSize(14)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('3. TEAM PLAYER AVERAGES', this.margin, currentY)
    
    currentY += 5
    this.doc.setFontSize(10)
    this.doc.setFont('helvetica', 'normal')
    
    const formattedStartDate = new Date(startDate).toLocaleDateString('en-US')
    const formattedEndDate = new Date(endDate).toLocaleDateString('en-US')
    this.doc.text(`Average statistics for Team ${teamId} players (${formattedStartDate} - ${formattedEndDate})`, this.margin, currentY)
    
    currentY += 10

    if (teamPlayerAverages.length === 0) {
      this.doc.setFont('helvetica', 'italic')
      this.doc.text('No team player data found for the specified period.', this.margin, currentY)
      currentY += 15
    } else {
      // Prepare table data
      const tableData = teamPlayerAverages.map(player => [
        player.playerId || '',
        (player.playerName || 'Unknown player').toString(),
        (player.matchesPlayed || 0).toString(),
        (player.avgPoints || 0).toFixed(1),
        (player.avgAssists || 0).toFixed(1),
        (player.avgFouls || 0).toFixed(1),
        (player.totalPointsSum || 0).toFixed(1),
        (player.totalAssistsSum || 0).toString(),
        (player.totalFoulsSum || 0).toString()
      ])

      // Calculate actual table width and center it
      const columnWidths = [18, 30, 16, 20, 18, 18, 18, 16, 16]; // Sum = 170mm
      const tableWidth = columnWidths.reduce((sum, width) => sum + width, 0);
      const tableStartX = (this.pageWidth - tableWidth) / 2;
      
      this.doc.autoTable({
        startY: currentY,
        head: [['Player ID', 'Player Name', 'Matches', 'Avg Points', 'Avg Assists', 'Avg Fouls', 'Total Points', 'Total Assists', 'Total Fouls']],
        body: tableData,
        theme: 'grid',
        headStyles: { 
          fillColor: [39, 174, 96], 
          textColor: 255, 
          fontStyle: 'bold', 
          fontSize: 8,
          halign: 'center' 
        },
        bodyStyles: { 
          fontSize: 7,
          halign: 'center'
        },
        alternateRowStyles: {
          fillColor: [200, 230, 201]
        },
        columnStyles: {
          0: { cellWidth: 18, halign: 'center' },
          1: { cellWidth: 30, halign: 'center' },
          2: { cellWidth: 16, halign: 'center' },
          3: { cellWidth: 20, halign: 'center', fontStyle: 'bold' },
          4: { cellWidth: 18, halign: 'center' },
          5: { cellWidth: 18, halign: 'center' },
          6: { cellWidth: 18, halign: 'center' },
          7: { cellWidth: 16, halign: 'center' },
          8: { cellWidth: 16, halign: 'center' }
        },
        margin: { left: tableStartX, right: tableStartX }
      })
    }

    return currentY
  }

  addReportFooter() {
    const finalY = this.doc.lastAutoTable ? this.doc.lastAutoTable.finalY : 200
    let footerY = Math.max(finalY + 30, this.pageHeight - 40)

    // If we're too close to the bottom, add a new page
    if (footerY > this.pageHeight - 40) {
      this.doc.addPage()
      footerY = this.pageHeight - 40
    }

    // Draw footer separator line
    this.doc.setLineWidth(0.5)
    this.doc.line(this.margin, footerY - 10, this.pageWidth - this.margin, footerY - 10)

    this.doc.setFontSize(10)
    this.doc.setFont('helvetica', 'normal')
    
    const reportDate = new Date().toLocaleString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    })
    
    this.doc.text(`InfluxDB Analytics Report Generated: ${reportDate}`, this.pageWidth / 2, footerY, { align: 'center' })
    
    // Add data source info
    this.doc.setFontSize(8)
    this.doc.setFont('helvetica', 'italic')
    this.doc.text('Data Source: InfluxDB Time-Series Database with Flux Query Language', this.pageWidth / 2, footerY + 8, { align: 'center' })
    this.doc.text('Report Types: Match Scoring Events | Player Event Counts | Team Player Averages', this.pageWidth / 2, footerY + 16, { align: 'center' })
  }

  formatDateForFilename(dateString) {
    const date = new Date(dateString)
    return date.toISOString().split('T')[0] // Returns YYYY-MM-DD format
  }

  /**
   * Convert milliseconds to MM:SS format
   * @param {number} milliseconds - Time in milliseconds
   * @returns {string} Formatted time as MM:SS
   */
  formatPeriodTime(milliseconds) {
    if (!milliseconds || milliseconds === 0) return '0:00'
    
    const totalSeconds = Math.floor(milliseconds / 1000)
    const minutes = Math.floor(totalSeconds / 60)
    const seconds = totalSeconds % 60
    
    return `${minutes}:${seconds.toString().padStart(2, '0')}`
  }
}

export default new InfluxReportService()