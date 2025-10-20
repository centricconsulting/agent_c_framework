# PLANE Tools - Rigorous Development & Testing Plan

**Date:** 2025-10-15  
**Branch:** rupert (development)  
**Goal:** Fix and verify ALL PLANE tools before server integration  
**Standard:** ZERO tolerance for errors that crash the server

---

## 🎯 Objectives

1. **Audit all existing PLANE code** for syntax, import, and logic errors
2. **Fix all issues** one file at a time with verification
3. **Test each component** in complete isolation
4. **Use clones and agents** for verification and testing
5. **Create comprehensive test suite** that validates everything
6. **Only integrate** after 100% verification
7. **Have rollback ready** at every step

---

## 📊 Current Status

**Location:** `//project/src/agent_c_tools/src/agent_c_tools/tools/plane/`  
**Status:** ⚠️ CONTAINS ERRORS - Server crashes on import  
**Branch:** rupert (safe for development)  
**Impact:** Cannot be integrated until fixed

---

## 🔍 Phase 1: Complete Code Audit

### Step 1.1: Inventory All Files
- [ ] List all PLANE files
- [ ] Document structure and dependencies
- [ ] Identify import chains

### Step 1.2: Syntax Check (Using Clone)
- [ ] Use clone to check EVERY file for syntax errors
- [ ] Verify: No `await` outside async functions
- [ ] Verify: No missing imports
- [ ] Verify: All classes properly defined

### Step 1.3: Import Chain Validation
- [ ] Test each import independently
- [ ] Verify: auth/ imports work
- [ ] Verify: client/ imports work
- [ ] Verify: tools/ imports work
- [ ] Verify: No circular dependencies

### Step 1.4: Dependency Check
- [ ] List all external dependencies
- [ ] Verify: All required packages installed
- [ ] Verify: Optional dependencies handled safely (Playwright)

---

## 🔧 Phase 2: Fix Issues (One at a Time)

### Step 2.1: Fix Core Infrastructure
**Order:** auth → client → toolsets

**For each file:**
1. Clone reviews the file
2. Identify ALL issues
3. Fix issues
4. Verify syntax with Python compiler
5. Test imports in isolation
6. Move to next file

**Files:**
- [ ] auth/cookie_manager.py
- [ ] auth/plane_session.py
- [ ] auth/cookie_refresh.py (make fully optional)
- [ ] client/plane_client.py

### Step 2.2: Fix Toolsets
**For each toolset:**
1. Verify imports work
2. Verify class structure correct
3. Verify Toolset.register() call present
4. Test tool methods don't crash

**Files:**
- [ ] tools/plane_projects.py
- [ ] tools/plane_issues.py
- [ ] tools/plane_search.py
- [ ] tools/plane_analytics.py
- [ ] tools/plane_issue_relations.py
- [ ] tools/plane_labels.py
- [ ] tools/plane_bulk.py

---

## 🧪 Phase 3: Comprehensive Testing

### Step 3.1: Unit Tests (Using Clone)
**Create test for each component:**
- [ ] test_cookie_manager.py - Encryption, storage, loading
- [ ] test_plane_session.py - HTTP session, error handling
- [ ] test_plane_client.py - API methods
- [ ] test_each_toolset.py - Each toolset independently

**Success Criteria:** ALL tests pass with 0 errors

### Step 3.2: Integration Tests
- [ ] Test import chain (can all modules import?)
- [ ] Test toolset registration (do they register?)
- [ ] Test with mock cookies (no real API needed)
- [ ] Test error handling (expired cookies, network errors)

### Step 3.3: Live API Tests
**ONLY after all above pass:**
- [ ] Test with real PLANE instance
- [ ] Verify all 33 tools work
- [ ] Test error scenarios
- [ ] Verify cleanup on errors

---

## ✅ Phase 4: Pre-Integration Verification

### Step 4.1: Server Import Test
**Before touching server:**
- [ ] Create standalone script that imports PLANE tools
- [ ] Run in same Python environment as server
- [ ] Verify NO import errors
- [ ] Verify NO crashes

### Step 4.2: Mock Integration Test
- [ ] Test in development environment
- [ ] Simulate server startup with PLANE tools
- [ ] Verify graceful handling if cookies missing
- [ ] Verify no crashes on initialization

### Step 4.3: Rollback Plan
- [ ] Document exact rollback steps
- [ ] Test rollback procedure
- [ ] Ensure can recover in < 1 minute

---

## 🚀 Phase 5: Controlled Integration

### Step 5.1: Preparation
- [ ] Create backup of working server
- [ ] Document exact state before integration
- [ ] Have rollback commands ready

### Step 5.2: Integration (With Safety Net)
- [ ] Add PLANE tools to __init__.py
- [ ] Test import: `python -c "from agent_c_tools.tools.plane import PlaneProjectTools"`
- [ ] If successful, proceed
- [ ] If fails, immediate rollback

### Step 5.3: Server Test
- [ ] Start server in test mode
- [ ] Verify startup successful
- [ ] Test basic PLANE tool call
- [ ] If ANY issue, immediate rollback

### Step 5.4: Live Verification
- [ ] Ask Rupert to list PLANE projects
- [ ] Verify response
- [ ] Test 2-3 more tools
- [ ] Only then declare success

---

## 🛡️ Safety Protocols

### Rule 1: No Direct Server Changes
- NEVER touch server without complete verification first
- Test everything in isolation
- Use development branch only

### Rule 2: Use Clones & Agents
- Clone for file analysis
- Clone for testing
- Possibly use Pyper for code quality review
- Possibly use Vera for test creation

### Rule 3: One Change at a Time
- Fix one file
- Test that file
- Verify it works
- Move to next file

### Rule 4: Always Have Rollback
- Document rollback before each change
- Test rollback procedure
- Keep working backup accessible

### Rule 5: Verify Before Integration
- 100% test coverage before touching server
- No assumptions
- No "it should work"
- Only "it DOES work, proven by tests"

---

## 🔄 Alternative: Clean Slate

If easier, we can:

### Option A: Complete Removal
- Remove ALL PLANE code
- Server works perfectly
- Rebuild from scratch later with proper process

### Option B: Incremental Rebuild
- Keep broken code isolated
- Rebuild piece by piece
- Test each piece thoroughly
- Only integrate when perfect

---

## 🤔 Your Choice

**What would you prefer?**

**A.** Remove PLANE tools entirely, get server working, rebuild later with proper process  
**B.** Keep PLANE code, I'll fix it methodically with clones/agents doing the heavy lifting  
**C.** Something else

**I will follow YOUR decision and be MUCH more careful this time.**
