const express = require('express');
const router = express.Router();

const driveController = require('../../controllers/settings/driveController');
const backupController = require('../../controllers/settings/backupController');
const integrationsController = require('../../controllers/settings/integrationsController');

router.get('/drive/usage', driveController.getDriveUsage);
router.get('/drive/timeline', driveController.getDriveTimeline);
router.get('/drive/last-sync', driveController.getLastSync);

router.post('/backup/email', backupController.sendBackupEmail);
router.post('/backup/manual', backupController.createManualBackup);
router.get('/backup/history', backupController.getBackupHistory);
router.get('/backup/download/:id', backupController.downloadBackup);
router.get('/backup/config', backupController.getBackupConfig);
router.post('/backup/config', backupController.updateBackupConfig);

router.get('/integrations', integrationsController.getIntegrationsStatus);

module.exports = router;
