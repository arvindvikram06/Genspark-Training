from dotenv import load_dotenv
from groq_service import generate_analysis

load_dotenv()

response = generate_analysis(
    "Explain REST API in 3 lines."
)

print(response)