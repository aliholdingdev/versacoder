---
title: İnsan Onayı ve Denetlenebilirlik
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-01.md
  - raw/versacoder-proje-planlamasi-ai-05.md
tags: [human-in-the-loop, audit, approval-gate]
---

# İnsan Onayı ve Denetlenebilirlik

**Human-in-the-Loop:** Architecture, Security, Data, Deployment, Auth, AI Provider, Model, Database kararları **kullanıcı belirlemeden gerçek kabul edilmez** (ai-01).

**Risk-based approval:** Her adımda durdurma değil, **risk seviyesine göre** kontrol. Düşük riskte akış devam eder; kritik noktada Approval Gate açılır (ai-01, ai-05 §31).

**Talep modeli:** `REQUEST → POLICY CHECK → APPROVAL → EXECUTION`. Policy Engine'den geçmeyen tool çağrısı çalışmaz; izinsiz dış erişim ve yıkıcı dosya işlemi yasaktır (ai-05 §31, ai-02).

**Auditability:** Hangi bağlam, agent, workflow, tool ve onayla yapıldığı geriye dönük incelenebilir olmalıdır. Session Registry SQLite + audit trail olarak saklanır (ai-01, ai-03).

**Bağımsız validation:** Agent "tamamlandı" dese de sonuç VERIFIED değildir; Reviewer onayı zorunludur (ai-05 §24).

**Kapılar (Versa Coder):** `GATE-003` = kod yazımı öncesi onay; `GATE-004` = ilk commit. GATE-003 öncesi `src/**` altına satır yazılmaz (ai-09 §5).

**Çıktı kutusu standardı:** `BAŞLIK / KAPSAM / KARAR / RİSK / KANIT`; KANIT somut değilse işlenmez (ai-09 §4, G5).

## İlgili Sayfalar
- [[agent-workflow]] — onayın akış içindeki yeri
- [[anti-halüsinasyon-kurallari]] — policy ve doğrulama kuralları
- [[versacoder-projesi]] — gate sisteminin ait olduğu proje
- [[session-wizard]] — "Onayla/Başlat" basamağı
