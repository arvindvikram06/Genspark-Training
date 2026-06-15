import imaplib
import email
from email.message import Message
from pathlib import Path
from typing import Optional, Tuple
from logger import logger
from config import Config

class EmailReader:
    """Handles reading emails via IMAP and extracting text attachments."""
    
    def __init__(self):
        self.mail = imaplib.IMAP4_SSL("imap.gmail.com")
        self.download_dir = Path("downloads")
        self.download_dir.mkdir(exist_ok=True)

    def connect(self) -> None:
        """Connects and logs into the Gmail IMAP server."""
        try:
            self.mail.login(Config.GMAIL_ADDRESS, Config.GMAIL_APP_PASSWORD)
            logger.info("Successfully connected to IMAP server.")
        except Exception as e:
            logger.error(f"IMAP login failed: {e}")
            raise

    def fetch_latest_unread_requirement(self) -> Tuple[Optional[str], Optional[str]]:
        """
        Fetches the latest unread email, looks for requirement.txt, 
        downloads it, and returns the client email and the text content.
        
        Returns:
            Tuple[Optional[str], Optional[str]]: (client_email, requirement_text)
        """
        try:
            self.mail.select("inbox")
            status, messages = self.mail.search(None, "UNSEEN")
            
            if status != "OK" or not messages[0]:
                logger.info("No unread messages found.")
                return None, None

            # Get the latest unread email ID
            email_ids = messages[0].split()
            latest_email_id = email_ids[-1]

            status, msg_data = self.mail.fetch(latest_email_id, "(RFC822)")
            if status != "OK":
                logger.error("Failed to fetch email data.")
                return None, None

            raw_email = msg_data[0][1]
            msg: Message = email.message_from_bytes(raw_email)
            client_email = email.utils.parseaddr(msg.get("From"))[1]

            # Parse attachments
            for part in msg.walk():
                if part.get_content_maintype() == "multipart":
                    continue
                if part.get("Content-Disposition") is None:
                    continue

                filename = part.get_filename()
                if filename and "requirement.txt" in filename.lower():
                    payload = part.get_payload(decode=True)
                    if not payload:
                        logger.warning("Found requirement.txt, but it is empty.")
                        return client_email, ""
                        
                    content = payload.decode("utf-8")
                    
                    # Save locally
                    filepath = self.download_dir / filename
                    with open(filepath, "w", encoding="utf-8") as f:
                        f.write(content)
                        
                    logger.info(f"Downloaded and extracted {filename} from {client_email}.")
                    return client_email, content

            logger.info(f"Unread email from {client_email} did not contain requirement.txt.")
            return client_email, None

        except Exception as e:
            logger.error(f"Error fetching emails: {e}")
            raise
        finally:
            self.mail.close()
            self.mail.logout()