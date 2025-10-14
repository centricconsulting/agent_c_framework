import os
import base64
import hashlib
import secrets

from urllib.parse import urlparse, parse_qs, urlencode

from selenium import webdriver
from selenium.common import TimeoutException
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC



def generate_code_verifier():
    return secrets.token_urlsafe(96)[:128]


def generate_code_challenge(code_verifier):
    sha256 = hashlib.sha256(code_verifier.encode('ascii')).digest()
    return base64.urlsafe_b64encode(sha256).decode('ascii').rstrip('=')


def get_oauth_token(client_type, **kwargs):
    # General Centric Information
    user_id = kwargs.get('centric_id', os.getenv('CENTRIC_ID'))
    user_pw = kwargs.get('centric_pw', os.getenv('CENTRIC_PW'))

    # For Dynamics
    client_id = kwargs.get('client_id')
    redirect_uri = kwargs.get('redirect_uri')
    scope = kwargs.get('dynamics_scope')

    # For Graph
    graph_client_id = kwargs.get('GRAPH_CLIENT_ID')

    # Launch Browser to manually authenticate
    options = webdriver.ChromeOptions()
    # options.add_argument("start-maximized")
    options.set_capability('goog:loggingPrefs', {'performance': 'ALL'})

    browser = webdriver.Chrome(options=options)

    # Construct the authorization URL
    if client_type == 'dynamics':
        auth_params = {
            'client_id': client_id,
            'redirect_uri': redirect_uri,
            'response_type': 'code',
            'scope': scope
        }
    elif client_type == 'graph':
        # TODO: Not Working
        code_verifier = generate_code_verifier()
        code_challenge = generate_code_challenge(code_verifier)
        auth_params = {
            'client_id': graph_client_id,
            'redirect_uri': redirect_uri,
            'response_type': 'code',
            'scope': 'https://graph.microsoft.com/Mail.Read',
            'code_challenge': code_challenge,
            'code_challenge_method': 'S256'
        }
    else:
        raise ValueError(f'Invalid client type {client_type}')

    auth_url = f"{os.getenv('AUTH_ENDPOINT')}?{urlencode(auth_params)}"

    # Navigate to authorizatioN URL
    browser.get(auth_url)
    # Enter credentials
    # Wait for the email input to be visible and interactable
    WebDriverWait(browser, 5).until(
        EC.visibility_of_element_located((By.NAME, 'loginfmt'))
    ).send_keys(user_id)

    # Wait for the 'Next' button to be clickable
    WebDriverWait(browser, 5).until(
        EC.element_to_be_clickable((By.XPATH, '//input[@value="Next"]'))
    ).click()

    # Wait for the password input to be visible and interactable
    WebDriverWait(browser, 5).until(
        EC.visibility_of_element_located((By.NAME, "passwd"))
    ).send_keys(user_pw)

    WebDriverWait(browser, 5).until(
        EC.element_to_be_clickable((By.XPATH, '//input[@value="Sign in"]'))
    ).click()

    try:
        # if the 'Yes' button is present, click it.
        # This yes button is the 'Do you want to automatically skip this page next time?' button
        WebDriverWait(browser, 15).until(
            EC.element_to_be_clickable((By.XPATH, '//input[@value="Yes"]'))
        ).click()
    except TimeoutException:
        # If the 'Yes' button is not present, just move on.
        pass

    WebDriverWait(browser, 25).until(
        lambda d: d.current_url.startswith(os.getenv('REDIRECT_URI'))
    )

    # Extract the authorization code from the URL
    current_url = browser.current_url
    parsed_url = urlparse(current_url)
    auth_code = parse_qs(parsed_url.query)['code'][0]

    # print(f"Authorization Code: {auth_code}")
    browser.close()

    if client_type == 'graph':
        return auth_code, code_verifier
    else:
        return auth_code
