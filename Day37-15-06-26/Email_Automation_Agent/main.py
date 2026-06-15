import sys
import time
from logger import logger
from email_reader import EmailReader
from groq_service import GroqAnalyzer
from parser import OutputParser
from email_sender import EmailSender
from config import Config

def process_latest_email() -> None:
    """Executes a single cycle of checking and processing emails."""
    reader = EmailReader()
    
    try:
        reader.connect()
        client_email, req_text = reader.fetch_latest_unread_requirement()

        if not client_email:
            # No unread emails, silently return and wait for next poll
            return
            
        if req_text is None:
            logger.info(f"Unread email from {client_email} ignored (no valid requirement.txt).")
            return
            
        if req_text.strip() == "":
            logger.warning(f"Requirement attachment from {client_email} is empty.")
            return

        logger.info(f"Processing requirement from {client_email}...")

        # Step 2: Analyze via LLM
        analyzer = GroqAnalyzer()
        analysis_data = analyzer.analyze_requirement(req_text)

        # Step 3: Parse and Save Artifacts
        parser = OutputParser()
        parser.generate_artifacts(analysis_data)

        # Step 4: Dispatch Emails
        sender = EmailSender()
        
        # Pass client_email so it appears in the internal email subject line
        sender.send_internal_report(client_email) 
        
        sender.send_client_clarification(client_email)
        
        logger.info(f"Successfully processed and replied to {client_email}.")

    except Exception as e:
        # We catch exceptions here so a single failure doesn't crash the whole service
        logger.error(f"Error during processing cycle: {e}", exc_info=True)

def main():
    logger.info(f"Starting AgentMail service. Listening for emails every {Config.POLL_INTERVAL} seconds...")

    try:
        while True:
            process_latest_email()
            # Sleep until the next polling cycle
            time.sleep(Config.POLL_INTERVAL)
            
    except KeyboardInterrupt:
        logger.info("AgentMail service stopped manually by user.")
        sys.exit(0)
    except Exception as e:
        logger.critical(f"AgentMail encountered a fatal error: {e}", exc_info=True)
        sys.exit(1)

if __name__ == "__main__":
    main()