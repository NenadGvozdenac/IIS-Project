#!/usr/bin/env python3
"""
Elasticsearch Report Generator
=====================================

This script demonstrates the report generation capabilities by:
1. Setting up Elasticsearch indexes
2. Generating dummy data
3. Performing simple and complex aggregations
4. Generating a PDF report

Author: Sports Hub Analytics Team
Date: September 29, 2025
"""

import requests
import json
import time
from datetime import datetime
from typing import Dict, Any, List, Optional
import sys

# Try to import PDF generation libraries
try:
    from reportlab.lib.pagesizes import letter, A4
    from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle
    from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
    from reportlab.lib.units import inch
    from reportlab.lib import colors
    PDF_AVAILABLE = True
except ImportError:
    print("⚠️  PDF libraries not available. Install with: pip install reportlab")
    PDF_AVAILABLE = False

class ElasticsearchReportGenerator:
    """Main class for generating Elasticsearch reports"""
    
    def __init__(self, base_url: str = "http://localhost:5008"):
        self.base_url = base_url
        self.session = requests.Session()
        self.session.headers.update({'Content-Type': 'application/json'})
        
    def print_section(self, title: str, char: str = "="):
        """Print a formatted section header"""
        print(f"\n{char * 60}")
        print(f"{title:^60}")
        print(f"{char * 60}")
        
    def make_request(self, method: str, endpoint: str, data: Optional[Dict] = None) -> Optional[Dict]:
        """Make HTTP request with error handling"""
        url = f"{self.base_url}{endpoint}"
        try:
            if method.upper() == "GET":
                response = self.session.get(url)
            elif method.upper() == "POST":
                response = self.session.post(url, json=data)
            elif method.upper() == "DELETE":
                response = self.session.delete(url)
            else:
                raise ValueError(f"Unsupported method: {method}")
                
            response.raise_for_status()
            
            # Handle both JSON responses and plain text
            try:
                return response.json()
            except:
                return {"message": response.text, "status_code": response.status_code}
                
        except requests.exceptions.ConnectionError:
            print(f"❌ Could not connect to {url}")
            print("   Make sure the Elasticsearch service is running on port 5008")
            return None
        except requests.exceptions.HTTPError as e:
            print(f"⚠️  HTTP Error {e.response.status_code}: {e.response.text}")
            if method.upper() == "DELETE" and e.response.status_code == 500:
                print("   (This might be normal if no indexes exist to delete)")
            return None
        except Exception as e:
            print(f"❌ Error making request to {endpoint}: {str(e)}")
            return None
    
    def step_1_delete_indexes(self) -> bool:
        """Step 1: Delete existing indexes"""
        self.print_section("STEP 1: Deleting Existing Indexes")
        result = self.make_request("DELETE", "/api/index/delete")
        
        if result:
            print("✅ Indexes deleted successfully")
            return True
        else:
            print("⚠️  Failed to delete indexes (possibly none exist) - continuing anyway")
            return True  # Continue even if deletion fails
    
    def step_2_create_indexes(self) -> bool:
        """Step 2: Create new indexes"""
        self.print_section("STEP 2: Creating New Indexes")
        result = self.make_request("POST", "/api/index/create")
        
        if result:
            print("✅ Indexes created successfully")
            return True
        else:
            print("❌ Failed to create indexes")
            return False
    
    def step_3_generate_dummy_data(self) -> bool:
        """Step 3: Generate dummy data"""
        self.print_section("STEP 3: Generating Dummy Data")
        result = self.make_request("POST", "/api/players/bulk/generate-dummy-data")
        
        if result:
            print("✅ Dummy data generated successfully")
            print(f"   📊 {result.get('playersCount', 'N/A')} players created")
            print(f"   📊 {result.get('sessionsCount', 'N/A')} sessions created")
            
            # Wait a bit for indexing
            print("⏳ Waiting for data to be indexed...")
            time.sleep(3)
            return True
        else:
            print("❌ Failed to generate dummy data")
            return False
    
    def step_4_simple_aggregation_1(self) -> Optional[List[Dict]]:
        """Simple Aggregation 1: Players with most points in a session"""
        self.print_section("SIMPLE AGGREGATION 1: Top Players by Points in Session")
        print("📋 Description: Retrieve players with the highest points scored in any single session")
        print("   This is a simple aggregation showing top performers by points")
        
        result = self.make_request("GET", "/api/players/aggregations/most-points-session?limit=5")
        
        if result:
            print("\n📊 Results:")
            print(f"{'Rank':<6}{'Player Name':<25}{'Session ID':<12}{'Points':<8}")
            print("-" * 55)
            
            for i, player in enumerate(result, 1):
                print(f"{i:<6}{player.get('playerFullName', 'N/A'):<25}{player.get('idSession', 'N/A'):<12}{player.get('points', 'N/A'):<8}")
            
            return result
        else:
            print("❌ Failed to get simple aggregation 1")
            return None
    
    def step_5_simple_aggregation_2(self) -> Optional[List[Dict]]:
        """Simple Aggregation 2: Top players with max points in the last year"""
        self.print_section("SIMPLE AGGREGATION 2: Top Players by Max Points (Last Year)")
        print("📋 Description: Retrieve players with highest maximum points achieved in the last year")
        print("   This shows peak performance across time periods")
        
        result = self.make_request("GET", "/api/players/aggregations/top-max-points-last-year")
        
        if result:
            print("\n📊 Results:")
            print(f"{'Rank':<6}{'Player Name':<25}{'Max Points':<12}{'Date':<12}")
            print("-" * 60)
            
            for i, player in enumerate(result, 1):
                session_date = player.get('sessionDate', '')
                if session_date:
                    try:
                        date_obj = datetime.fromisoformat(session_date.replace('Z', '+00:00'))
                        formatted_date = date_obj.strftime('%Y-%m-%d')
                    except:
                        formatted_date = session_date[:10] if len(session_date) >= 10 else session_date
                else:
                    formatted_date = 'N/A'
                    
                print(f"{i:<6}{player.get('playerFullName', 'N/A'):<25}{player.get('maxPoints', 'N/A'):<12}{formatted_date:<12}")
            
            return result
        else:
            print("❌ Failed to get simple aggregation 2")
            return None
    
    def step_6_complex_aggregation(self) -> Optional[List[Dict]]:
        """Complex Aggregation: Players with most playoff minutes by nationality"""
        self.print_section("COMPLEX AGGREGATION: Playoff Minutes by Nationality")
        print("📋 Description: Complex query retrieving players with most playoff minutes for a specific nationality")
        print("   This demonstrates filtering by session type AND nationality with aggregation")
        
        # Try different nationalities to find data
        nationalities = ["USA", "Serbia", "Spain", "France", "Germany"]
        
        for nationality in nationalities:
            print(f"\n🔍 Searching for players from {nationality}...")
            result = self.make_request("GET", f"/api/players/aggregations/playoff-minutes-by-nationality?nationality={nationality}&limit=5")
            
            if result and len(result) > 0:
                print(f"\n📊 Results for {nationality}:")
                print(f"{'Rank':<6}{'Player Name':<25}{'Nationality':<15}{'Playoff Minutes':<15}")
                print("-" * 70)
                
                for i, player in enumerate(result, 1):
                    print(f"{i:<6}{player.get('playerFullName', 'N/A'):<25}{player.get('nationality', 'N/A'):<15}{player.get('totalPlayoffMinutes', 'N/A'):<15}")
                
                return result
            else:
                print(f"   No data found for {nationality}")
        
        print("❌ No playoff data found for any nationality")
        return None
    
    def step_7_saga_transaction_test(self) -> Optional[Dict]:
        """Test Elastic Orchestrator Service Player Update Transaction"""
        self.print_section("SAGA TRANSACTION TEST: Player Update via Orchestrator")
        print("📋 Description: Test the elastic orchestrator service saga pattern for player updates")
        print("   This demonstrates distributed transaction management across microservices")
        
        # Define the orchestrator service URL (port 5010)
        orchestrator_url = "http://localhost:5010"
        
        # Check if orchestrator service is available
        print("\n🔍 Checking Elastic Orchestrator Service availability...")
        try:
            response = requests.get(f"{orchestrator_url}/api/saga/transactions", timeout=5)
            if response.status_code != 200:
                print(f"⚠️  Orchestrator service responded with status {response.status_code}")
                print("   Make sure the elastic_orchestrator_service is running on port 5010")
                return None
        except requests.exceptions.ConnectionError:
            print("❌ Could not connect to Elastic Orchestrator Service")
            print("   Make sure the elastic_orchestrator_service is running on port 5010")
            return None
        except Exception as e:
            print(f"❌ Error connecting to orchestrator: {str(e)}")
            return None
        
        print("✅ Elastic Orchestrator Service is available")
        
        # Create a test player update request
        test_player_data = {
            "player": {
                "idPlayer": 1,
                "name": "Test",
                "surname": "Player",
                "fullName": "Test Player Updated",
                "birthday": "1995-06-15T00:00:00Z",
                "nationality": "USA",
                "position": "Point Guard",
                "physicalMetrics": [
                    {
                        "verticalJump": 85,
                        "fatPercentage": 8,
                        "benchPressWeight": 120,
                        "squatWeight": 180,
                        "sprintSpeed": 95,
                        "weight": 85,
                        "height": 188,
                        "wingspan": 195,
                        "dateOfMeasurement": "2025-09-29T00:00:00Z"
                    }
                ]
            }
        }
        
        print("\n🔄 Starting player update saga transaction...")
        print(f"   Player ID: {test_player_data['player']['idPlayer']}")
        print(f"   Player Name: {test_player_data['player']['fullName']}")
        
        # Start the saga transaction
        try:
            response = requests.post(
                f"{orchestrator_url}/api/saga/update-player",
                json=test_player_data,
                headers={'Content-Type': 'application/json'},
                timeout=10
            )
            
            if response.status_code == 202:  # Accepted
                transaction = response.json()
                transaction_id = transaction.get('transactionId')
                print(f"✅ Saga transaction started successfully")
                print(f"   Transaction ID: {transaction_id}")
                print(f"   Status: {transaction.get('status', 'N/A')}")
                
                # Wait a bit for transaction to process
                print("\n⏳ Waiting for transaction to process...")
                time.sleep(3)
                
                # Check transaction status
                status_response = requests.get(
                    f"{orchestrator_url}/api/saga/transactions/{transaction_id}",
                    timeout=5
                )
                
                if status_response.status_code == 200:
                    updated_transaction = status_response.json()
                    print(f"\n📊 Transaction Status Update:")
                    print(f"   Transaction ID: {updated_transaction.get('transactionId', 'N/A')}")
                    print(f"   Status: {updated_transaction.get('status', 'N/A')}")
                    print(f"   Created At: {updated_transaction.get('createdAt', 'N/A')}")
                    print(f"   Completed At: {updated_transaction.get('completedAt', 'N/A')}")
                    
                    # Show saga steps
                    steps = updated_transaction.get('steps', [])
                    if steps:
                        print(f"\n📋 Saga Steps ({len(steps)} total):")
                        print(f"{'Step':<30}{'Status':<15}{'Executed At':<20}")
                        print("-" * 70)
                        for step in steps:
                            executed_at = step.get('executedAt', 'N/A')
                            if executed_at and executed_at != 'N/A':
                                try:
                                    date_obj = datetime.fromisoformat(executed_at.replace('Z', '+00:00'))
                                    executed_at = date_obj.strftime('%Y-%m-%d %H:%M:%S')
                                except:
                                    pass
                            print(f"{step.get('stepName', 'N/A'):<30}{step.get('status', 'N/A'):<15}{executed_at:<20}")
                    
                    # Show error message if any
                    error_msg = updated_transaction.get('errorMessage')
                    if error_msg:
                        print(f"\n⚠️  Error Message: {error_msg}")
                    
                    return updated_transaction
                else:
                    print(f"⚠️  Could not get specific transaction status: {status_response.status_code}")
                    print("   Trying to get all transactions to find our transaction...")
                    
                    # Fallback: Get all transactions and find ours
                    try:
                        all_transactions_response = requests.get(
                            f"{orchestrator_url}/api/saga/transactions",
                            timeout=5
                        )
                        
                        if all_transactions_response.status_code == 200:
                            all_transactions = all_transactions_response.json()
                            if isinstance(all_transactions, list):
                                # Find our transaction by ID
                                our_transaction = None
                                for tx in all_transactions:
                                    if str(tx.get('transactionId', '')).lower() == str(transaction_id).lower():
                                        our_transaction = tx
                                        break
                                
                                if our_transaction:
                                    print(f"✅ Found transaction in all transactions list")
                                    print(f"\n📊 Transaction Status Update:")
                                    print(f"   Transaction ID: {our_transaction.get('transactionId', 'N/A')}")
                                    print(f"   Status: {our_transaction.get('status', 'N/A')}")
                                    print(f"   Created At: {our_transaction.get('createdAt', 'N/A')}")
                                    print(f"   Completed At: {our_transaction.get('completedAt', 'N/A')}")
                                    
                                    # Show saga steps
                                    steps = our_transaction.get('steps', [])
                                    if steps:
                                        print(f"\n📋 Saga Steps ({len(steps)} total):")
                                        print(f"{'Step':<30}{'Status':<15}{'Executed At':<20}")
                                        print("-" * 70)
                                        for step in steps:
                                            executed_at = step.get('executedAt', 'N/A')
                                            if executed_at and executed_at != 'N/A':
                                                try:
                                                    date_obj = datetime.fromisoformat(executed_at.replace('Z', '+00:00'))
                                                    executed_at = date_obj.strftime('%Y-%m-%d %H:%M:%S')
                                                except:
                                                    pass
                                            print(f"{step.get('stepName', 'N/A'):<30}{step.get('status', 'N/A'):<15}{executed_at:<20}")
                                    
                                    # Show error message if any
                                    error_msg = our_transaction.get('errorMessage')
                                    if error_msg:
                                        print(f"\n⚠️  Error Message: {error_msg}")
                                    
                                    return our_transaction
                                else:
                                    print(f"⚠️  Transaction {transaction_id} not found in transactions list")
                                    print(f"   Available transactions: {len(all_transactions)}")
                                    return transaction
                            else:
                                print(f"⚠️  Unexpected response format from all transactions endpoint")
                                return transaction
                        else:
                            print(f"⚠️  Could not get all transactions: {all_transactions_response.status_code}")
                            return transaction
                    except Exception as e:
                        print(f"⚠️  Error getting all transactions: {str(e)}")
                        return transaction
                    
            else:
                print(f"❌ Failed to start saga transaction: {response.status_code}")
                print(f"   Response: {response.text}")
                return None
                
        except requests.exceptions.Timeout:
            print("❌ Request timeout - saga transaction may still be processing")
            return None
        except Exception as e:
            print(f"❌ Error starting saga transaction: {str(e)}")
            return None
    
    def generate_pdf_report(self, simple_agg1: List[Dict], simple_agg2: List[Dict], complex_agg: List[Dict]):
        """Generate PDF report with results"""
        if not PDF_AVAILABLE:
            print("\n📄 PDF generation skipped - reportlab not installed")
            return
            
        self.print_section("GENERATING PDF REPORT")
        
        filename = f"elasticsearch_report_{datetime.now().strftime('%Y%m%d_%H%M%S')}.pdf"
        doc = SimpleDocTemplate(filename, pagesize=A4)
        styles = getSampleStyleSheet()
        story = []
        
        # Title
        title_style = ParagraphStyle(
            'CustomTitle',
            parent=styles['Heading1'],
            fontSize=18,
            spaceAfter=30,
            alignment=1  # Center alignment
        )
        story.append(Paragraph("Sports Hub Analytics - Elasticsearch Report", title_style))
        story.append(Spacer(1, 20))
        
        # Report info
        info_style = styles['Normal']
        story.append(Paragraph(f"Generated on: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}", info_style))
        story.append(Paragraph("This report demonstrates Elasticsearch aggregation capabilities", info_style))
        story.append(Spacer(1, 30))
        
        # Simple Aggregation 1
        story.append(Paragraph("1. Simple Aggregation: Top Players by Points in Session", styles['Heading2']))
        story.append(Paragraph("Description: Players with the highest points scored in any single session", styles['Normal']))
        story.append(Spacer(1, 10))
        
        if simple_agg1:
            data1 = [['Rank', 'Player Name', 'Session ID', 'Points']]
            for i, player in enumerate(simple_agg1, 1):
                data1.append([
                    str(i),
                    player.get('playerFullName', 'N/A'),
                    str(player.get('idSession', 'N/A')),
                    str(player.get('points', 'N/A'))
                ])
            
            table1 = Table(data1)
            table1.setStyle(TableStyle([
                ('BACKGROUND', (0, 0), (-1, 0), colors.grey),
                ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
                ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
                ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
                ('FONTSIZE', (0, 0), (-1, 0), 12),
                ('BOTTOMPADDING', (0, 0), (-1, 0), 12),
                ('BACKGROUND', (0, 1), (-1, -1), colors.beige),
                ('GRID', (0, 0), (-1, -1), 1, colors.black)
            ]))
            story.append(table1)
        else:
            story.append(Paragraph("No data available", styles['Normal']))
        
        story.append(Spacer(1, 30))
        
        # Simple Aggregation 2
        story.append(Paragraph("2. Simple Aggregation: Top Players by Max Points (Last Year)", styles['Heading2']))
        story.append(Paragraph("Description: Players with highest maximum points achieved in the last year", styles['Normal']))
        story.append(Spacer(1, 10))
        
        if simple_agg2:
            data2 = [['Rank', 'Player Name', 'Max Points', 'Date']]
            for i, player in enumerate(simple_agg2, 1):
                session_date = player.get('sessionDate', '')
                if session_date:
                    try:
                        date_obj = datetime.fromisoformat(session_date.replace('Z', '+00:00'))
                        formatted_date = date_obj.strftime('%Y-%m-%d')
                    except:
                        formatted_date = session_date[:10] if len(session_date) >= 10 else session_date
                else:
                    formatted_date = 'N/A'
                
                data2.append([
                    str(i),
                    player.get('playerFullName', 'N/A'),
                    str(player.get('maxPoints', 'N/A')),
                    formatted_date
                ])
            
            table2 = Table(data2)
            table2.setStyle(TableStyle([
                ('BACKGROUND', (0, 0), (-1, 0), colors.grey),
                ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
                ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
                ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
                ('FONTSIZE', (0, 0), (-1, 0), 12),
                ('BOTTOMPADDING', (0, 0), (-1, 0), 12),
                ('BACKGROUND', (0, 1), (-1, -1), colors.beige),
                ('GRID', (0, 0), (-1, -1), 1, colors.black)
            ]))
            story.append(table2)
        else:
            story.append(Paragraph("No data available", styles['Normal']))
        
        story.append(Spacer(1, 30))
        
        # Complex Aggregation
        story.append(Paragraph("3. Complex Aggregation: Playoff Minutes by Nationality", styles['Heading2']))
        story.append(Paragraph("Description: Complex query retrieving players with most playoff minutes for a specific nationality", styles['Normal']))
        story.append(Spacer(1, 10))
        
        if complex_agg:
            data3 = [['Rank', 'Player Name', 'Nationality', 'Playoff Minutes']]
            for i, player in enumerate(complex_agg, 1):
                data3.append([
                    str(i),
                    player.get('playerFullName', 'N/A'),
                    player.get('nationality', 'N/A'),
                    str(player.get('totalPlayoffMinutes', 'N/A'))
                ])
            
            table3 = Table(data3)
            table3.setStyle(TableStyle([
                ('BACKGROUND', (0, 0), (-1, 0), colors.grey),
                ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
                ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
                ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
                ('FONTSIZE', (0, 0), (-1, 0), 12),
                ('BOTTOMPADDING', (0, 0), (-1, 0), 12),
                ('BACKGROUND', (0, 1), (-1, -1), colors.beige),
                ('GRID', (0, 0), (-1, -1), 1, colors.black)
            ]))
            story.append(table3)
        else:
            story.append(Paragraph("No data available", styles['Normal']))
        
        # Footer
        story.append(Spacer(1, 30))
        footer_style = ParagraphStyle(
            'Footer',
            parent=styles['Normal'],
            fontSize=10,
            textColor=colors.grey
        )
        story.append(Paragraph("Generated by Sports Hub Analytics - Elasticsearch Report Generator", footer_style))
        
        try:
            doc.build(story)
            print(f"✅ PDF report generated: {filename}")
        except Exception as e:
            print(f"❌ Error generating PDF: {str(e)}")
    
    def run_full_report(self):
        """Run the complete report generation process"""
        print("🚀 Sports Hub Analytics - Elasticsearch Report Generator")
        print("=" * 60)
        print("This script will:")
        print("1. Delete existing indexes")
        print("2. Create new indexes") 
        print("3. Generate dummy data")
        print("4. Run simple aggregations (2)")
        print("5. Run complex aggregation (1)")
        print("6. Test saga transaction (player update)")
        print("7. Generate PDF report")
        print()
        
        # Check service availability
        print("🔍 Checking Elasticsearch service availability...")
        test_result = self.make_request("GET", "/api/players?take=1")
        if test_result is None:
            print("❌ Cannot connect to Elasticsearch service. Please ensure it's running.")
            return False
        print("✅ Elasticsearch service is available")
        
        # Step 1: Delete indexes
        if not self.step_1_delete_indexes():
            return False
        
        # Step 2: Create indexes
        if not self.step_2_create_indexes():
            return False
        
        # Step 3: Generate dummy data
        if not self.step_3_generate_dummy_data():
            return False
        
        # Step 4: Simple aggregation 1
        simple_agg1 = self.step_4_simple_aggregation_1()
        
        # Step 5: Simple aggregation 2
        simple_agg2 = self.step_5_simple_aggregation_2()
        
        # Step 6: Complex aggregation
        complex_agg = self.step_6_complex_aggregation()
        
        # Step 7: Saga transaction test
        saga_result = self.step_7_saga_transaction_test()
        
        # Generate PDF report (only with aggregation results, not saga test)
        if any([simple_agg1, simple_agg2, complex_agg]):
            self.generate_pdf_report(
                simple_agg1 or [],
                simple_agg2 or [],
                complex_agg or []
            )
        
        # Summary
        self.print_section("REPORT GENERATION COMPLETE")
        print("📊 Summary:")
        print(f"   Simple Aggregation 1: {'✅ Success' if simple_agg1 else '❌ Failed'}")
        print(f"   Simple Aggregation 2: {'✅ Success' if simple_agg2 else '❌ Failed'}")
        print(f"   Complex Aggregation:  {'✅ Success' if complex_agg else '❌ Failed'}")
        print(f"   Saga Transaction:     {'✅ Success' if saga_result else '❌ Failed'}")
        
        if PDF_AVAILABLE:
            print("   PDF Report: ✅ Generated")
        else:
            print("   PDF Report: ⚠️  Skipped (install reportlab)")
        
        print("\n🎉 Report generation process completed!")
        return True


def main():
    """Main function"""
    print("Starting Elasticsearch Report Generator...")
    
    # Check if service URL is provided
    service_url = "http://localhost:5008"
    if len(sys.argv) > 1:
        service_url = sys.argv[1]
    
    generator = ElasticsearchReportGenerator(service_url)
    
    try:
        success = generator.run_full_report()
        if success:
            print("\n✅ All operations completed successfully!")
        else:
            print("\n❌ Some operations failed. Check the output above.")
            sys.exit(1)
    except KeyboardInterrupt:
        print("\n\n⚠️  Operation interrupted by user")
        sys.exit(1)
    except Exception as e:
        print(f"\n❌ Unexpected error: {str(e)}")
        sys.exit(1)


if __name__ == "__main__":
    main()