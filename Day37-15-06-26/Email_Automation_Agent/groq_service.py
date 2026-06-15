import json
from openai import OpenAI
from pathlib import Path
from typing import Dict, Any
from logger import logger
from config import Config

class GroqAnalyzer:
    """Handles communication with the Groq API using the OpenAI SDK."""
    
    def __init__(self):
        self.client = OpenAI(
            api_key=Config.GROQ_API_KEY,
             base_url="https://api.groq.com/openai/v1")
        self.model = "llama-3.3-70b-versatile"
        self.prompt_path = Path("prompt.txt")

    def _load_prompt(self) -> str:
        """Loads the system prompt from the prompt.txt file."""
        if not self.prompt_path.exists():
            error_msg = "prompt.txt file is missing."
            logger.error(error_msg)
            raise FileNotFoundError(error_msg)
        return self.prompt_path.read_text(encoding="utf-8")

    def analyze_requirement(self, text: str) -> Dict[str, Any]:
        """
        Sends the requirement text to Groq and expects a JSON response.
        
        Args:
            text (str): The raw requirement text from the client.
            
        Returns:
            Dict[str, Any]: The parsed JSON response matching the requested schema.
        """
        logger.info("Sending requirement text to Groq API for analysis...")
        system_prompt = self._load_prompt()
        
        try:
            response = self.client.chat.completions.create(
                model=self.model,
                messages=[
                    {"role": "system", "content": system_prompt},
                    {"role": "user", "content": f"Analyze the following requirements:\n\n{text}"}
                ],
                response_format={"type": "json_object"},
                temperature=0.2
            )
            
            raw_content = response.choices[0].message.content
            if not raw_content:
                raise ValueError("Received empty response from LLM.")
                
            parsed_json = json.loads(raw_content)
            logger.info("Successfully received and parsed JSON from Groq API.")
            return parsed_json
            
        except json.JSONDecodeError as e:
            logger.error(f"Failed to parse LLM response as JSON: {e}")
            raise
        except Exception as e:
            logger.error(f"Groq API call failed: {e}")
            raise