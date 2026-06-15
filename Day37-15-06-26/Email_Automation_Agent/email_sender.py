import smtplib
from email.message import EmailMessage
from pathlib import Path
from logger import logger
from config import Config

class EmailSender:
    """Handles sending emails via Gmail SMTP, including file attachments."""

    def __init__(self):
        self.smtp_server = "smtp.gmail.com"
        self.smtp_port = 587

    def _send_raw_email(self, msg: EmailMessage, to_address: str) -> None:
        """Helper method to execute the SMTP connection and send."""
        try:
            with smtplib.SMTP(self.smtp_server, self.smtp_port) as server:
                server.starttls()
                server.login(Config.GMAIL_ADDRESS, Config.GMAIL_APP_PASSWORD)
                server.send_message(msg)
            logger.info(f"Email sent successfully to {to_address}.")
        except Exception as e:
            logger.error(f"Failed to send email to {to_address}: {e}")
            raise

    def send_internal_report(self, client_email: str) -> None:
        """Creates a formatted email and attaches the Markdown analysis."""
        filepath = Path("output/requirement_analysis.md")
        if not filepath.exists():
            logger.error("requirement_analysis.md not found. Cannot send internal email.")
            return

        msg = EmailMessage()
        msg['Subject'] = f"New Requirement Analysis: {client_email}"
        msg['From'] = Config.GMAIL_ADDRESS
        msg['To'] = Config.INTERNAL_TEAM_EMAIL

        # Formatted body for the internal team
        body = f"""Hello Team,

A new requirement document has been received from {client_email} and analyzed.

Please find the complete Requirement Analysis attached to this email. The system has automatically isolated the functional/non-functional requirements, identified risks, and outlined assumptions.

Best regards,
AgentMail Automation
"""
        msg.set_content(body)

        # Read the markdown file and attach it
        with open(filepath, 'rb') as f:
            file_data = f.read()

        msg.add_attachment(
            file_data,
            maintype='text',
            subtype='markdown',
            filename=filepath.name
        )

        self._send_raw_email(msg, Config.INTERNAL_TEAM_EMAIL)

    def send_client_clarification(self, client_email: str) -> None:
        """Reads the generated client questions and emails them to the client."""
        filepath = Path("output/client_questions.txt")
        if not filepath.exists():
            logger.error("client_questions.txt not found. Cannot send client email.")
            return

        content = filepath.read_text(encoding="utf-8")
        
        msg = EmailMessage()
        msg['Subject'] = "Clarifications regarding your requirements"
        msg['From'] = Config.GMAIL_ADDRESS
        msg['To'] = client_email
        msg.set_content(content)

        self._send_raw_email(msg, client_email)