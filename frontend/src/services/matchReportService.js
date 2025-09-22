import jsPDF from 'jspdf'
import 'jspdf-autotable'
import { getUserData } from './auth_service.js'

export class MatchReportService {
  constructor() {
    this.doc = null
    this.pageWidth = 210 // A4 width in mm
    this.pageHeight = 297 // A4 height in mm
    this.margin = 20
  }

  generateMatchReport(matchData, ourTeamPlayers, opponentTeamPlayers, ourTeamStats, opponentTeamStats) {
    this.doc = new jsPDF()
    this.matchData = matchData // Store for use in other methods
    console.log('Match Data:', matchData)
    console.log('Our Team Players:', ourTeamPlayers)
    console.log('Opponent Team Players:', opponentTeamPlayers)
    console.log('Our Team Stats:', ourTeamStats)
    console.log('Opponent Team Stats:', opponentTeamStats)
    // Add header
    this.addReportHeader(matchData)
    
    // Add match score
    this.addMatchScore(matchData)
    
    // Add team statistics
    this.addTeamStatistics(ourTeamStats, opponentTeamStats, matchData)
    
    // Add player statistics
    this.addPlayerStatistics(ourTeamPlayers, opponentTeamPlayers, matchData)
    
    // Add footer
    this.addReportFooter()
    
    // Download the PDF
    const fileName = `Match_Report_${matchData.name.replace(/\s+/g, '_')}_${this.formatDateForFilename(matchData.scheduledAt)}.pdf`
    this.doc.save(fileName)
  }

  addReportHeader(matchData) {
    const centerX = this.pageWidth / 2

    // Main title
    this.doc.setFontSize(20)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('BASKETBALL LEAGUE', centerX, 25, { align: 'center' })
    
    this.doc.setFontSize(12)
    this.doc.setFont('helvetica', 'normal')
    this.doc.text('Professional Basketball Association', centerX, 32, { align: 'center' })
    
    this.doc.setFontSize(16)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('OFFICIAL MATCH REPORT', centerX, 45, { align: 'center' })
    
    // Draw header separator line
    this.doc.setLineWidth(0.5)
    this.doc.line(this.margin, 50, this.pageWidth - this.margin, 50)
    
    // Match details immediately under header
    const detailsYStart = 58
    this.doc.setFontSize(10)
    this.doc.setFont('helvetica', 'normal')
    const matchDate = new Date(matchData.scheduledAt)
    const formattedDate = matchDate.toLocaleDateString('en-US', { 
      weekday: 'long', 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    })
    const formattedTime = matchDate.toLocaleTimeString('en-US', { 
      hour: '2-digit', 
      minute: '2-digit',
      hour12: true 
    })

    const detailsCenterX = centerX - 25
    this.doc.text(`Date: ${formattedDate}`, detailsCenterX, detailsYStart)
    this.doc.text(`Time: ${formattedTime}`, detailsCenterX, detailsYStart + 7)
    this.doc.text(`Venue: ${matchData.hall}, ${matchData.city}`, detailsCenterX, detailsYStart + 14)
    this.doc.text(`Competition: ${matchData.competitionName || 'Regular Season'}`, detailsCenterX, detailsYStart + 21)
  }

  addMatchScore(matchData) {
    const centerX = this.pageWidth / 2
    let currentY = 90 // moved down to make room for header details

    // Score display box - larger and centered
    const boxWidth = 160
    const boxHeight = 40
    const boxX = (this.pageWidth - boxWidth) / 2

    this.doc.setLineWidth(1)
    this.doc.rect(boxX, currentY, boxWidth, boxHeight)

    // "VS" and team names on the same level
    this.doc.setFontSize(14)
    this.doc.setFont('helvetica', 'bold')
    const teamNamesY = currentY + 10

    // Team names on left and right sides, aligned with VS
    this.doc.setFontSize(15)
    this.doc.text('KK Partizan', boxX + (boxWidth / 4), teamNamesY, { align: 'center' })
    this.doc.text('VS', centerX, teamNamesY, { align: 'center' })
    this.doc.text(matchData.teamName || 'KK Crvena Zvezda', boxX + (3 * boxWidth / 4), teamNamesY, { align: 'center' })

    // "Final Score" label
    this.doc.setFontSize(12)
    this.doc.setFont('helvetica', 'normal')
    this.doc.text('Final Score', centerX, currentY + 22, { align: 'center' })

    // Centered final score with larger font
    this.doc.setFontSize(25)
    this.doc.setFont('helvetica', 'bold')
    const finalScore = `${matchData.ourPoints || 0} - ${matchData.opponentPoints || 0}`
    this.doc.text(finalScore, centerX, currentY + 34, { align: 'center' })

    // leave space after score box
    currentY += boxHeight + 20
  }

  addTeamStatistics(ourTeamStats, opponentTeamStats, matchData) {
    let currentY = 147

    this.doc.setFontSize(14)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('TEAM STATISTICS', this.pageWidth / 2, currentY, { align: 'center' })
    currentY += 8

    // Helper function to safely get stat values
    const getStat = (stats, path) => {
      try {
        const keys = path.split('.')
        let value = stats
        for (const key of keys) {
          value = value?.[key]
        }
        return value ?? 0
      } catch (e) {
        return 0
      }
    }

    // Team statistics table
    const tableData = [
      ['Field Goals', 
       `${getStat(ourTeamStats, 'fieldGoals.made')}/${getStat(ourTeamStats, 'fieldGoals.attempts')} (${getStat(ourTeamStats, 'fieldGoals.percentage')}%)`, 
       `${getStat(opponentTeamStats, 'fieldGoals.made')}/${getStat(opponentTeamStats, 'fieldGoals.attempts')} (${getStat(opponentTeamStats, 'fieldGoals.percentage')}%)`],
      ['2-Point Field Goals', 
       `${getStat(ourTeamStats, 'twoPointers.made')}/${getStat(ourTeamStats, 'twoPointers.attempts')} (${getStat(ourTeamStats, 'twoPointers.percentage')}%)`, 
       `${getStat(opponentTeamStats, 'twoPointers.made')}/${getStat(opponentTeamStats, 'twoPointers.attempts')} (${getStat(opponentTeamStats, 'twoPointers.percentage')}%)`],
      ['3-Point Field Goals', 
       `${getStat(ourTeamStats, 'threePointers.made')}/${getStat(ourTeamStats, 'threePointers.attempts')} (${getStat(ourTeamStats, 'threePointers.percentage')}%)`, 
       `${getStat(opponentTeamStats, 'threePointers.made')}/${getStat(opponentTeamStats, 'threePointers.attempts')} (${getStat(opponentTeamStats, 'threePointers.percentage')}%)`],
      ['Free Throws', 
       `${getStat(ourTeamStats, 'freeThrows.made')}/${getStat(ourTeamStats, 'freeThrows.attempts')} (${getStat(ourTeamStats, 'freeThrows.percentage')}%)`, 
       `${getStat(opponentTeamStats, 'freeThrows.made')}/${getStat(opponentTeamStats, 'freeThrows.attempts')} (${getStat(opponentTeamStats, 'freeThrows.percentage')}%)`],
      ['Rebounds (Off/Def)', 
       `${getStat(ourTeamStats, 'rebounds.total')} (${getStat(ourTeamStats, 'rebounds.offensive')}/${getStat(ourTeamStats, 'rebounds.defensive')})`, 
       `${getStat(opponentTeamStats, 'rebounds.total')} (${getStat(opponentTeamStats, 'rebounds.offensive')}/${getStat(opponentTeamStats, 'rebounds.defensive')})`],
      ['Assists', getStat(ourTeamStats, 'assists').toString(), getStat(opponentTeamStats, 'assists').toString()],
      ['Blocks', getStat(ourTeamStats, 'blocks').toString(), getStat(opponentTeamStats, 'blocks').toString()],
      ['Personal Fouls', getStat(ourTeamStats, 'fouls').toString(), getStat(opponentTeamStats, 'fouls').toString()]
    ]
    console.log('STAGOD - Team stats processed successfully')

    // Calculate table width and center it
    const tableWidth = 170
    const tableStartX = (this.pageWidth - tableWidth) / 2
    
    this.doc.autoTable({
      startY: currentY,
      head: [['', 'KK PARTIZAN', matchData.teamName || 'OPPONENT TEAM']],
      body: tableData,
      theme: 'grid',
      headStyles: { fillColor: [41, 128, 185], textColor: 255, fontStyle: 'bold', fontSize: 10, halign: 'center' },
      bodyStyles: { fontSize: 9 },
      columnStyles: {
        0: { cellWidth: 50, fontStyle: 'bold' },
        1: { cellWidth: 60, halign: 'center' },
        2: { cellWidth: 60, halign: 'center' }
      },
      margin: { left: tableStartX, right: tableStartX }
    })
  }

  addPlayerStatistics(ourTeamPlayers, opponentTeamPlayers, matchData) {
    let currentY = this.doc.lastAutoTable.finalY + 20

    // Check if we need a new page
    if (currentY > 220) {
      this.doc.addPage()
      currentY = 30
    }

    // Our team player statistics
    this.doc.setFontSize(12)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('PLAYER STATISTICS - KK PARTIZAN', this.pageWidth / 2, currentY, { align: 'center' })
    
    currentY += 10

    const ourTeamTableData = ourTeamPlayers.map(player => {
      // Calculate efficiency: PTS + REB + AST + STL + BLK - (FGA - FGM) - (FTA - FTM) - TO
      const fgm = (player?.twoPointers?.made || 0) + (player?.threePointers?.made || 0)
      const fga = (player?.twoPointers?.attempts || 0) + (player?.threePointers?.attempts || 0)
      const ftm = player?.freeThrows?.made || 0
      const fta = player?.freeThrows?.attempts || 0
      const pts = player?.points || 0
      const reb = (player?.rebounds?.offensive || 0) + (player?.rebounds?.defensive || 0)
      const ast = player?.assists || 0
      const stl = player?.steals || 0
      const blk = player?.blocks || 0
      
      const efficiency = pts + reb + ast + stl + blk - (fga - fgm) - (fta - ftm)
      
      return [
        player?.jerseyNumber || '',
        `${player?.firstName || ''} ${player?.lastName || ''}` || 'Unknown Player',
        `${fgm}/${fga}`,
        `${player?.twoPointers?.made || 0}/${player?.twoPointers?.attempts || 0}`,
        `${player?.threePointers?.made || 0}/${player?.threePointers?.attempts || 0}`,
        `${player?.freeThrows?.made || 0}/${player?.freeThrows?.attempts || 0}`,
        `${player?.rebounds?.offensive || 0}/${player?.rebounds?.defensive || 0}`,
        player?.assists || 0,
        player?.steals || 0,
        player?.blocks || 0,
        player?.fouls || 0,
        efficiency,
        player?.points || 0
      ]
    })

    this.doc.autoTable({
      startY: currentY,
      head: [['#', 'PLAYER', 'FG', '2P', '3P', 'FT', 'REB', 'AST', 'STL', 'BLK', 'PF', 'EFF', 'PTS']],
      body: ourTeamTableData,
      theme: 'grid',
      headStyles: { 
        fillColor: [52, 73, 94], 
        textColor: 255, 
        fontStyle: 'bold', 
        fontSize: 7, 
        halign: 'center' 
      },
      bodyStyles: { 
        fontSize: 7,
        halign: 'center'
      },
      alternateRowStyles: {
        fillColor: [200, 200, 200], // Light gray for alternating rows
        fontSize: 7,
        halign: 'center'
      },
      columnStyles: {
        0: { cellWidth: 10, halign: 'center' },
        1: { cellWidth: 28, halign: 'left' },
        2: { cellWidth: 12, halign: 'center' },
        3: { cellWidth: 12, halign: 'center' },
        4: { cellWidth: 12, halign: 'center' },
        5: { cellWidth: 12, halign: 'center' },
        6: { cellWidth: 15, halign: 'center' },
        7: { cellWidth: 10, halign: 'center' },
        8: { cellWidth: 10, halign: 'center' },
        9: { cellWidth: 10, halign: 'center' },
        10: { cellWidth: 10, halign: 'center' },
        11: { cellWidth: 12, halign: 'center' },
        12: { cellWidth: 12, halign: 'center' }
      },
      styles: {
        lineColor: [0, 0, 0], // Black grid lines
        lineWidth: 0.1
      },
      margin: { left: this.margin, right: this.margin }
    })

    currentY = this.doc.lastAutoTable.finalY + 20

    // Check if we need a new page for opponent statistics
    if (currentY > 200) {
      this.doc.addPage()
      currentY = 30
    }

    // Opponent team player statistics
    this.doc.setFontSize(12)
    this.doc.setFont('helvetica', 'bold')
    const opponentTeamName = opponentTeamPlayers.length > 0 ? 
      (opponentTeamPlayers[0].teamName || 'OPPONENT TEAM') : 'OPPONENT TEAM'
    this.doc.text(`PLAYER STATISTICS - ${opponentTeamName.toUpperCase()}`, this.pageWidth / 2, currentY, { align: 'center' })
    
    currentY += 10

    const opponentTableData = opponentTeamPlayers.map(player => {
      // Calculate efficiency: PTS + REB + AST + STL + BLK - (FGA - FGM) - (FTA - FTM) - TO
      const fgm = (player?.twoPointers?.made || 0) + (player?.threePointers?.made || 0)
      const fga = (player?.twoPointers?.attempts || 0) + (player?.threePointers?.attempts || 0)
      const ftm = player?.freeThrows?.made || 0
      const fta = player?.freeThrows?.attempts || 0
      const pts = player?.points || 0
      const reb = (player?.rebounds?.offensive || 0) + (player?.rebounds?.defensive || 0)
      const ast = player?.assists || 0
      const stl = player?.steals || 0
      const blk = player?.blocks || 0
      
      const efficiency = pts + reb + ast + stl + blk - (fga - fgm) - (fta - ftm)
      
      return [
        player?.jerseyNumber || '',
        `${player?.firstName || ''} ${player?.lastName || ''}` || 'Unknown Player',
        `${fgm}/${fga}`,
        `${player?.twoPointers?.made || 0}/${player?.twoPointers?.attempts || 0}`,
        `${player?.threePointers?.made || 0}/${player?.threePointers?.attempts || 0}`,
        `${player?.freeThrows?.made || 0}/${player?.freeThrows?.attempts || 0}`,
        `${player?.rebounds?.offensive || 0}/${player?.rebounds?.defensive || 0}`,
        player?.assists || 0,
        player?.steals || 0,
        player?.blocks || 0,
        player?.fouls || 0,
        efficiency,
        player?.points || 0
      ]
    })

    this.doc.autoTable({
      startY: currentY,
      head: [['#', 'PLAYER', 'FG', '2P', '3P', 'FT', 'REB', 'AST', 'STL', 'BLK', 'PF', 'EFF', 'PTS']],
      body: opponentTableData,
      theme: 'grid',
      headStyles: { 
        fillColor: [52, 73, 94], 
        textColor: 255, 
        fontStyle: 'bold', 
        fontSize: 7, 
        halign: 'center' 
      },
      bodyStyles: { 
        fontSize: 7,
        halign: 'center'
      },
      alternateRowStyles: {
        fillColor: [200, 200, 200], // Light gray for alternating rows
        fontSize: 7,
        halign: 'center'
      },
      columnStyles: {
        0: { cellWidth: 10, halign: 'center' },
        1: { cellWidth: 28, halign: 'left' },
        2: { cellWidth: 12, halign: 'center' },
        3: { cellWidth: 12, halign: 'center' },
        4: { cellWidth: 12, halign: 'center' },
        5: { cellWidth: 12, halign: 'center' },
        6: { cellWidth: 15, halign: 'center' },
        7: { cellWidth: 10, halign: 'center' },
        8: { cellWidth: 10, halign: 'center' },
        9: { cellWidth: 10, halign: 'center' },
        10: { cellWidth: 10, halign: 'center' },
        11: { cellWidth: 12, halign: 'center' },
        12: { cellWidth: 12, halign: 'center' }
      },
      styles: {
        lineColor: [0, 0, 0], // Black grid lines
        lineWidth: 0.1
      },
      margin: { left: this.margin, right: this.margin }
    })
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
    
    this.doc.text(`Report Generated: ${reportDate}`, this.pageWidth / 2, footerY, { align: 'center' })
    
    // Get current analyst information
    const userData = getUserData()
    const analystName = userData ? `${userData.userName} (${userData.userEmail})` : 'System Automated Report'
    this.doc.text(`Match Analyst: ${analystName}`, this.pageWidth / 2, footerY + 8, { align: 'center' })
  }

  formatDateForFilename(dateString) {
    const date = new Date(dateString)
    return date.toISOString().split('T')[0] // Returns YYYY-MM-DD format
  }
}

export default new MatchReportService()