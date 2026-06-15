import os
from dotenv import load_dotenv
from logger import logger

load_dotenv()

class Config:
    """Holds configuration securely loaded from environment variables."""
    GMAIL_ADDRESS: str = os.getenv("GMAIL_ADDRESS", "")
    GMAIL_APP_PASSWORD: str = os.getenv("GMAIL_APP_PASSWORD", "")
    INTERNAL_TEAM_EMAIL: str = os.getenv("INTERNAL_TEAM_EMAIL", "")
    GROQ_API_KEY: str = os.getenv("GROQ_API_KEY", "")
    
    # New: Polling interval with a default fallback of 60 seconds
    POLL_INTERVAL: int = int(os.getenv("POLL_INTERVAL", 60))
    
    @classmethod
    def validate(cls) -> None:
        """Validates that all required environment variables are present."""
        missing = []
        if not cls.GMAIL_ADDRESS: missing.append("GMAIL_ADDRESS")
        if not cls.GMAIL_APP_PASSWORD: missing.append("GMAIL_APP_PASSWORD")
        if not cls.INTERNAL_TEAM_EMAIL: missing.append("INTERNAL_TEAM_EMAIL")
        if not cls.GROQ_API_KEY: missing.append("GROQ_API_KEY")
        
        if missing:
            error_msg = f"Missing environment variables: {', '.join(missing)}"
            logger.error(error_msg)
            raise EnvironmentError(error_msg)

# Run validation on import
Config.validate()