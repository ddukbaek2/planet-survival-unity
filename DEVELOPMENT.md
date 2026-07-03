# 우주해병 키우기 — 개발 가이드 / 픽스 히스토리

이 문서는 다른 개발 장비에서도 현재와 동일한 방식으로 개발/빌드/배포할 수 있도록,
지금까지 확정된 사양·프로세스·기술적 픽스와 그 이유를 정리한 것입니다.
새 장비에서 작업을 시작하기 전에 이 문서를 먼저 읽어 주세요.

---

## 1. 프로젝트 개요

- 장르: 탑다운 뷰 아이들(뱀서라이크식) 슈터. 플레이어는 이동만 조작하고, 가장 가까운 적을 자동 조준·자동 발사로 처치. 피격으로 체력 0 이 되면 게임 오버.
- 엔진: Unity 6000.3.18f1 / URP / 신규 Input System / TextMeshPro.
- 타깃: 스팀(가로 1280x800 기준) + 웹(WebGL) 포트폴리오 배포. 모바일 브라우저(세로) 도 지원.
- 씬 구성: `Assets/Scenes/Title.unity`(빌드 인덱스 0) → `Assets/Scenes/Game.unity`(인덱스 1).
  타이틀에서 화면을 한 번 클릭하면 Game 씬 로드.

---

## 2. 확정된 게임 사양 (사용자 오더)

- 조작: 화면 드래그 이동. 누른 지점을 기준으로 드래그 방향으로 이동속도 등속 이동. 회전 보간 없이 즉시 그 방향을 바라봄.
- 드래그 시 화면에 조이스틱 표시(하얀 원 스프라이트 기반, 베이스+노브). 기능은 드래그 이동과 동일, 시각화만 추가.
- 모든 캐릭터/발사체는 큐브. (플레이어=큐브, 적=큐브, 발사체=큐브)
- 전부 흰색 + 블룸. 엔티티는 흰색 이미시브(발광), 바닥만 어두운 "황량한 행성" 텍스처(무발광, 이음새 없는 타일러블).
- 그림자: 라이트 실시간 그림자 사용 금지. 원형(방사형) 텍스처 데칼을 각 큐브 밑 바닥에 깔아 처리.
- 체력바: 플레이어·모든 적. 흰색 정사각형 텍스처 기반 월드스페이스 바(배경+채움).
- 스폰: 뱀서라이크처럼 대량(버스트 스폰, 시간에 따라 간격↓·개수↑).
- 폰트: 반드시 `Assets/Resources/Font/경기천년바탕_Regular SDF.asset` (TMP SDF) 를 그대로 사용. 새 폰트 애셋을 만들지 말 것. 모든 UI 는 TMP(uGUI). (IMGUI/OnGUI 금지 — SDF 폰트를 못 씀)
- 폰트 크기 조정은 스케일이 아니라 fontSize 로 한다.
- 크기 기준: 적/발사체는 절반 스케일(적 0.5, 발사체 0.175), 플레이어는 그 2배(1.0). 맵은 기본의 3배(Ground scale 15 = 150x150, 플레이어 이동 경계 x±31.5 / z±18.9).
- 카메라: 플레이어를 부드럽게(SmoothDamp) 추적, 맵 경계 밖은 보이지 않게 클램프. 탑다운 orthographic(size 7).
- 타이틀: 게임명 "우주해병 키우기", 우하단에 현재 배포한 git 커밋 해시 표시.

---

## 3. 빌드/배포 프로세스 (★ 다른 장비에서도 반드시 동일하게)

매 빌드·배포마다 아래 순서를 지킨다.

1. `git add -A` → 커밋 → `git push` (branch `main`, 원격 `origin`). 커밋 메시지 끝에 Co-Authored-By 라인.
2. `git rev-parse --short HEAD` 로 방금 푸시한 짧은 해시를 얻는다.
3. `Assets/Resources/version.txt` 에 `"<shorthash> · <날짜>"` 를 기록한다.
   - 타이틀 씬의 `TitleController` 가 `Resources.Load<TextAsset>("version")` 로 읽어 우하단에 표시.
   - version.txt 는 커밋 이후 작성하므로 해당 커밋에는 포함되지 않는다(다음 커밋에 포함). 표시 해시 = 방금 푸시한 커밋.
4. Unity 에서 WebGL 빌드 → `D:/.../Builds/WebGL` (clean build).
5. 배포 대상으로 미러 복사.

### 빌드 설정 (고정)

- 압축: **Disabled** (Brotli/`.br` 금지). 이유: 배포 nginx 가 `.br` 을 `Content-Encoding: br` 헤더 없이 서빙 → 브라우저가 압축 해제 실패. (사용자가 BR 사용을 명시적으로 금지)
- WebGL 템플릿: `PROJECT:Landscape` (`Assets/WebGLTemplates/Landscape`).
- 빌드 씬: 인덱스 0 = Title, 1 = Game.
- 출력: `Builds/WebGL` (경로는 장비별로 다를 수 있음 — 프로젝트 루트의 `Builds/WebGL` 기준).

### 배포 대상

- `\\DS216PLUSII\web\ddukbaek2\portfolio\planet-survival-unity`
- `robocopy <src> <dst> /MIR` 사용(기존 파일 정리 포함). robocopy 종료코드 8 미만은 성공.
- 접속 URL 예: `http://DS216PLUSII/ddukbaek2/portfolio/planet-survival-unity/`

### 서버 서빙 확인(문제 진단용)

정상 MIME (확인됨): `.js` → application/javascript, `.wasm` → application/wasm, `.data` → application/octet-stream.

---

## 4. WebGL 반응형 레이아웃 (Landscape 템플릿)

`Assets/WebGLTemplates/Landscape/index.html` 의 CSS 로 처리.

- 가로(또는 정사각) 화면: `width:100vw`, `height: min(100vw/1.6, vh*100)`. 가로를 항상 꽉 채우고(좌우 필러박스 없음), 남는 위아래는 동일한 레터박스(세로 중앙 정렬). 16:10 보다 넓은 화면은 좌우 바 없이 가로로 더 넓게 렌더(잘림 방지).
- 세로 화면(`@media (max-aspect-ratio: 1/1)`): `width:100vw`, `height: vh*100`. 1280x800 기준을 버리고 화면 전체를 채워 세로전용 게임처럼 표시.
- `--vh` 는 JS 로 `window.innerHeight` 기반 계산(모바일 주소창 대응).
- devicePixelRatio: `window.devicePixelRatio` 그대로 사용해 내부 렌더 버퍼를 브라우저 실제 해상도에 맞춘다(특히 세로모드 저해상도 방지).
- 로딩 실패 시 화면에 오류 메시지를 표시(iOS 등 콘솔 접근이 어려운 환경 진단용).

---

## 5. 주요 기술적 픽스와 이유

- Brotli 로딩 실패 → 압축 Disabled 로 전환(위 3장 참고).
- WebGL 에서 Bloom 셰이더 스트리핑 방지: URP 애셋 HDR 켜기, 카메라 `renderPostProcessing`+`allowHDR`, `Hidden/Universal Render Pipeline/UberPost` 와 `.../Bloom` 셰이더를 Graphics 의 Always Included Shaders 에 등록.
- 이미시브 발광 + Bloom: 엔티티 머티리얼 `_EMISSION` 활성 + `_EmissionColor = white*2.5`, Global Volume 에 Bloom(threshold ~1.05).
- 그림자/체력바/조이스틱 텍스처는 코드로 생성(`Textures/ShadowBlob.png`, `WhiteSquare.png`, `WhiteCircle.png`, `PlanetGround.png`).
- 스프라이트로 쓸 텍스처는 임포터에서 `textureType=Sprite` 뿐 아니라 `spriteImportMode=Single` 까지 명시해야 Sprite 서브에셋이 생성됨. 리임포트 직후 `LoadAssetAtPath<Sprite>` 가 null 일 수 있으니 `ForceSynchronousImport` 후 재조회.
- 바닥 타일 이음새 제거: 격자 해시를 period 로 wrap 하는 주기적(타일러블) value noise 로 생성. (주의: 네 모서리를 전체 u,v 로 이중선형 블렌드하는 방식은 디테일이 뭉개지므로 쓰지 말 것)
- 조이스틱은 Screen Space Overlay 캔버스(ConstantPixelSize) 라 포스트프로세싱(블룸) 영향 없음. 불투명(투명도 없음)으로 선명하게.
- 체력바(HealthBar): 캐릭터 스케일이 바뀌어도 위치가 맞도록 `worldOffset` 을 `anchor.lossyScale` 로 스케일. 부모 요(yaw) 회전에 흔들리지 않게 LateUpdate 에서 월드 회전을 평면(Euler 90,0,0) 고정.

---

## 6. 코드/에셋 컨벤션 (C#)

- CLAUDE.md 의 규칙 중 C# 에 적용 가능한 것만 따른다(JS 전용 규칙인 `System.` 접두사, `#` private, NodeLayout, vanilla.js 는 제외).
- 변수명 축약 금지(full name), 모든 제어문 중괄호 강제, `else`/`catch` 개행, 한 줄 한 구문, 문자열은 쌍따옴표.
- 스크립트: `Assets/Scripts/`. 프리팹: `Assets/Prefabs/`. 머티리얼: `Assets/Materials/`. 텍스처: `Assets/Textures/`.

---

## 7. Unity MCP 작업 참고

- `create_script` 는 `batch_execute` 안에서 미지원 — 개별 호출.
- 새 커스텀 컴포넌트를 `manage_components`/`manage_gameobject` 로 붙일 때 컴파일 직후 타입 해석이 실패할 수 있음 → `execute_code` 로 `AddComponent(System.Type.GetType("<Name>, Assembly-CSharp"))` 사용이 안정적.
- 비활성 오브젝트는 `GameObject.Find` 로 못 찾음 → 활성 부모/캔버스에서 `transform.Find` 로 접근.
- 스크립트/에셋 변경 후 `read_console` 로 컴파일 에러 확인. 스크린샷은 사용하지 않음(상태 조회로 검증).
