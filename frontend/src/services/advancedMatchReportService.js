import jsPDF from 'jspdf'
import 'jspdf-autotable'
import axios from 'axios'
import { getUserData } from './auth_service.js'
import { MATCHES_URL } from './const_service.js'

export class AdvancedMatchReportService {
  constructor() {
    this.doc = null
    this.pageWidth = 210 // A4 width in mm
    this.pageHeight = 297 // A4 height in mm
    this.margin = 20
  }

  /**
   * Generate advanced match report by fetching data from backend API
   * @param {number} matchId - The ID of the match
   */
  async generateAdvancedMatchReport(matchId) {
    try {
      // Fetch data from backend API
      const backendData = await this.getCompleteMatchReport(matchId)
      const transformedData = this.transformForPdfGeneration(backendData)
      
      // Generate PDF with the fetched data
      this.doc = new jsPDF()
      this.matchData = transformedData.matchData
      
      console.log('Match Data:', transformedData.matchData)
      console.log('Our Team Players:', transformedData.ourTeamPlayers)
      console.log('Opponent Team Players:', transformedData.opponentTeamPlayers)
      console.log('Our Team Stats:', transformedData.ourTeamStats)
      console.log('Opponent Team Stats:', transformedData.opponentTeamStats)
      
      // Add header
      this.addReportHeader(transformedData.matchData)
      
      // Add match score
      this.addMatchScore(transformedData.matchData)
      
      // Add team statistics
      this.addTeamStatistics(transformedData.ourTeamStats, transformedData.opponentTeamStats, transformedData.matchData)
      
      // Add player statistics
      this.addPlayerStatistics(transformedData.ourTeamPlayers, transformedData.opponentTeamPlayers, transformedData.matchData)
      
      // Add player of the match
      this.addPlayerOfTheMatch(transformedData.playerOfTheMatch)
      
      // Add footer
      this.addReportFooter()
      
      // Download the PDF
      const fileName = `Advanced_Match_Report_${transformedData.matchData.name.replace(/\s+/g, '_')}_${this.formatDateForFilename(transformedData.matchData.scheduledAt)}.pdf`
      this.doc.save(fileName)
      
    } catch (error) {
      console.error('Error generating advanced match report:', error)
      throw error
    }
  }

  /**
   * Fetch complete match report from backend API
   * @param {number} matchId - The ID of the match
   * @returns {Promise<Object>} Complete match report data
   */
  async getCompleteMatchReport(matchId) {
    try {
      const response = await axios.get(`${MATCHES_URL}/match/${matchId}/complete-report`)
      console.log('RESPONSE: ', response)
      if (response.status !== 200) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const result = response.data

      if (!result.isSuccess) {
        throw new Error(result.error || 'Failed to fetch match report')
      }

      return result.value
    } catch (error) {
      console.error('Error fetching complete match report:', error)
      throw error
    }
  }

  /**
   * Transform backend response to format expected by PDF generator
   * @param {Object} backendData - Data from backend API
   * @returns {Object} Transformed data for PDF generation
   */
  transformForPdfGeneration(backendData) {
    try {
      const {
        generalInfo,
        ourTeamStats,
        opponentTeamStats,
        ourPlayersStats,
        opponentPlayersStats
      } = backendData

      // Transform general match data
      const matchData = {
        name: generalInfo?.matchName || 'Unknown Match',
        scheduledAt: generalInfo?.scheduledAt || new Date().toISOString(),
        hall: generalInfo?.hall || 'Unknown Hall',
        city: generalInfo?.city || 'Unknown City',
        state: generalInfo?.state || 'Unknown State',
        ourPoints: generalInfo?.finalScoreOur || 0,
        opponentPoints: generalInfo?.finalScoreOpponent || 0,
        teamName: opponentTeamStats?.teamName || 'Opponent Team'
      }

      // Transform our team players and sort by jersey number
      const ourTeamPlayers = (ourPlayersStats || [])
        .map(player => ({
          jerseyNumber: player.playerId || 0, // Using playerId as jersey number for now
          firstName: (player.playerName || '').split(' ')[0] || '',
          lastName: (player.playerName || '').split(' ').slice(1).join(' ') || '',
          points: player.totalPoints || 0,
          assists: player.totalAssists || 0,
          rebounds: {
            offensive: player.offensiveRebounds || 0,
            defensive: player.defensiveRebounds || 0
          },
          steals: player.totalSteals || 0,
          blocks: player.totalBlocks || 0,
          fouls: player.totalFouls || 0,
          twoPointers: {
            made: player.shooting2PMade || 0,
            attempts: player.shooting2PAttempted || 0,
            percentage: player.shooting2PPercentage || 0
          },
          threePointers: {
            made: player.shooting3PMade || 0,
            attempts: player.shooting3PAttempted || 0,
            percentage: player.shooting3PPercentage || 0
          },
          freeThrows: {
            made: player.freeThrowsMade || 0,
            attempts: player.freeThrowsAttempted || 0,
            percentage: player.freeThrowPercentage || 0
          },
          efficiency: player.efficiencyRating || 0,
          teamName: 'KK Partizan'
        }))
        .sort((a, b) => a.jerseyNumber - b.jerseyNumber)

      // Transform opponent team players and sort by jersey number
      const opponentTeamPlayers = (opponentPlayersStats || [])
        .map(player => ({
          jerseyNumber: player.playerId || 0,
          firstName: (player.playerName || '').split(' ')[0] || '',
          lastName: (player.playerName || '').split(' ').slice(1).join(' ') || '',
          points: player.totalPoints || 0,
          assists: player.totalAssists || 0,
          rebounds: {
            offensive: player.offensiveRebounds || 0,
            defensive: player.defensiveRebounds || 0
          },
          steals: player.totalSteals || 0,
          blocks: player.totalBlocks || 0,
          fouls: player.totalFouls || 0,
          twoPointers: {
            made: player.shooting2PMade || 0,
            attempts: player.shooting2PAttempted || 0,
            percentage: player.shooting2PPercentage || 0
          },
          threePointers: {
            made: player.shooting3PMade || 0,
            attempts: player.shooting3PAttempted || 0,
            percentage: player.shooting3PPercentage || 0
          },
          freeThrows: {
            made: player.freeThrowsMade || 0,
            attempts: player.freeThrowsAttempted || 0,
            percentage: player.freeThrowPercentage || 0
          },
          efficiency: player.efficiencyRating || 0,
          teamName: opponentTeamStats?.teamName || 'Opponent Team'
        }))
        .sort((a, b) => a.jerseyNumber - b.jerseyNumber)

      // Transform our team stats
      const ourTeamStatsTransformed = {
        fieldGoals: {
          made: ourTeamStats?.totalFieldGoalsMade || 0,
          attempts: ourTeamStats?.totalFieldGoalsAttempted || 0,
          percentage: ourTeamStats?.fieldGoalPercentage || 0
        },
        twoPointers: {
          made: ourTeamStats?.total2PMade || 0,
          attempts: ourTeamStats?.total2PAttempted || 0,
          percentage: ourTeamStats?.twoPointPercentage || 0
        },
        threePointers: {
          made: ourTeamStats?.total3PMade || 0,
          attempts: ourTeamStats?.total3PAttempted || 0,
          percentage: ourTeamStats?.threePointPercentage || 0
        },
        freeThrows: {
          made: ourTeamStats?.totalFreeThrowsMade || 0,
          attempts: ourTeamStats?.totalFreeThrowsAttempted || 0,
          percentage: ourTeamStats?.freeThrowPercentage || 0
        },
        rebounds: {
          total: ourTeamStats?.totalRebounds || 0,
          offensive: ourTeamStats?.totalOffensiveRebounds || 0,
          defensive: ourTeamStats?.totalDefensiveRebounds || 0
        },
        assists: ourTeamStats?.totalAssists || 0,
        blocks: ourTeamStats?.totalBlocks || 0,
        fouls: ourTeamStats?.totalFouls || 0,
        steals: ourTeamStats?.totalSteals || 0
      }

      // Transform opponent team stats
      const opponentTeamStatsTransformed = {
        fieldGoals: {
          made: opponentTeamStats?.totalFieldGoalsMade || 0,
          attempts: opponentTeamStats?.totalFieldGoalsAttempted || 0,
          percentage: opponentTeamStats?.fieldGoalPercentage || 0
        },
        twoPointers: {
          made: opponentTeamStats?.total2PMade || 0,
          attempts: opponentTeamStats?.total2PAttempted || 0,
          percentage: opponentTeamStats?.twoPointPercentage || 0
        },
        threePointers: {
          made: opponentTeamStats?.total3PMade || 0,
          attempts: opponentTeamStats?.total3PAttempted || 0,
          percentage: opponentTeamStats?.threePointPercentage || 0
        },
        freeThrows: {
          made: opponentTeamStats?.totalFreeThrowsMade || 0,
          attempts: opponentTeamStats?.totalFreeThrowsAttempted || 0,
          percentage: opponentTeamStats?.freeThrowPercentage || 0
        },
        rebounds: {
          total: opponentTeamStats?.totalRebounds || 0,
          offensive: opponentTeamStats?.totalOffensiveRebounds || 0,
          defensive: opponentTeamStats?.totalDefensiveRebounds || 0
        },
        assists: opponentTeamStats?.totalAssists || 0,
        blocks: opponentTeamStats?.totalBlocks || 0,
        fouls: opponentTeamStats?.totalFouls || 0,
        steals: opponentTeamStats?.totalSteals || 0
      }

      // Find player of the match (highest efficiency)
      const allPlayers = [...ourTeamPlayers, ...opponentTeamPlayers]
      const playerOfTheMatch = allPlayers.reduce((best, current) => {
        return (current.efficiency > best.efficiency) ? current : best
      }, allPlayers[0] || { efficiency: 0, firstName: 'N/A', lastName: '', teamName: 'N/A' })

      return {
        matchData,
        ourTeamPlayers,
        opponentTeamPlayers,
        ourTeamStats: ourTeamStatsTransformed,
        opponentTeamStats: opponentTeamStatsTransformed,
        playerOfTheMatch
      }
    } catch (error) {
      console.error('Error transforming data for PDF generation:', error)
      throw error
    }
  }

  addReportHeader(matchData) {
    const centerX = this.pageWidth / 2

    // Main title
    this.doc.setFontSize(20)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('KK PARTIZAN', centerX, 25, { align: 'center' })
    
    this.doc.setFontSize(12)
    this.doc.setFont('helvetica', 'normal')
    this.doc.text('Professional Basketball Association', centerX, 32, { align: 'center' })
    
    this.doc.setFontSize(10)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('ADVANCED MATCH REPORT', centerX, 45, { align: 'center' })
    
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
    this.doc.text(`Venue: ${matchData.state}, ${matchData.city} - ${matchData.hall}`, detailsCenterX, detailsYStart + 14)
  }

  addMatchScore(matchData) {
    const centerX = this.pageWidth / 2
    let currentY = 85 // moved down to make room for additional header info

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
    let currentY = 157

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
    console.log('Advanced report - Team stats processed successfully')

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
    
    currentY += 5

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
      
      //const efficiency = pts + reb + ast + stl + blk - (fga - fgm) - (fta - ftm)
      
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
        player?.efficiency,
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

    currentY = this.doc.lastAutoTable.finalY + 15

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
    
    currentY += 5

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
      
      //const efficiency = pts + reb + ast + stl + blk - (fga - fgm) - (fta - ftm)
      
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
        player?.efficiency,
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

  addPlayerOfTheMatch(playerOfTheMatch) {
    let currentY = this.doc.lastAutoTable.finalY + 15

    // Check if we need a new page
    if (currentY > 240) {
      this.doc.addPage()
      currentY = 30
    }

    // Player of the match section
    this.doc.setFontSize(16)
    this.doc.setFont('helvetica', 'bold')
    this.doc.text('PLAYER OF THE MATCH', this.pageWidth / 2, currentY, { align: 'center' })
    
    currentY += 5

    // Player details box
    const boxWidth = 100
    const boxHeight = 20
    const boxX = (this.pageWidth - boxWidth) / 2

    this.doc.setLineWidth(1)
    this.doc.rect(boxX, currentY, boxWidth, boxHeight)

    // Player name and team
    this.doc.setFontSize(15)
    this.doc.setFont('helvetica', 'bold')
    const playerName = `${playerOfTheMatch.firstName} ${playerOfTheMatch.lastName}`.trim()
    this.doc.text(playerName, this.pageWidth / 2, currentY + 8, { align: 'center' })
    
    // Efficiency rating
    this.doc.setFontSize(12)
    this.doc.setFont('helvetica', 'normal')
    this.doc.text(`Efficiency Rating: ${playerOfTheMatch.efficiency.toFixed(1)}`, this.pageWidth / 2, currentY + 15, { align: 'center' })
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
    
    this.doc.text(`Advanced Report Generated: ${reportDate}`, this.pageWidth / 2, footerY, { align: 'center' })
    
    // Get current analyst information
    const userData = getUserData()
    const analystName = userData ? `${userData.userName} (${userData.userEmail})` : 'Advanced Analytics System'
    this.doc.text(`Match Analyst: ${analystName}`, this.pageWidth / 2, footerY + 8, { align: 'center' })
    
    // Add data source info
    this.doc.setFontSize(8)
    this.doc.setFont('helvetica', 'italic')
    this.doc.text('Data Source: PostgreSQL PL/SQL Advanced Analytics Functions', this.pageWidth / 2, footerY + 16, { align: 'center' })
  }

  formatDateForFilename(dateString) {
    const date = new Date(dateString)
    return date.toISOString().split('T')[0] // Returns YYYY-MM-DD format
  }
}

export default new AdvancedMatchReportService()