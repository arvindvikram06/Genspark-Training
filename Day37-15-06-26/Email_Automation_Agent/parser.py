import json
from pathlib import Path
from typing import Dict, Any
from logger import logger

class OutputParser:
    """Handles formatting and saving the LLM JSON output to disk."""
    
    def __init__(self):
        self.output_dir = Path("output")
        self.output_dir.mkdir(exist_ok=True)

    def generate_artifacts(self, data: Dict[str, Any]) -> None:
        """
        Takes the JSON dictionary and saves the generated Markdown report 
        and client email text file.
        """
        try:
            # 1. Save the beautifully formatted Markdown Report directly
            # We pull the 'markdown_report' key that the LLM generated for us
            md_content = data.get('markdown_report', '# Error\nReport generation failed.')
            
            with open(self.output_dir / "requirement_analysis.md", "w", encoding="utf-8") as f:
                f.write(md_content)

            # 2. Save Client Questions TXT for the email sender
            # We pull the 'client_email_draft' key that the LLM generated for us
            client_email = data.get('client_email_draft', 'Please see attached questions.')
            
            with open(self.output_dir / "client_questions.txt", "w", encoding="utf-8") as f:
                f.write(client_email)

            logger.info(f"Successfully saved parsed artifacts to {self.output_dir}/")
            
        except Exception as e:
            logger.error(f"Error parsing and saving artifacts: {e}")
            raise